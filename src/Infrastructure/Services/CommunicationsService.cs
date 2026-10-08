using Notify.Client;
using System.Diagnostics.CodeAnalysis;

namespace Cfo.Cats.Infrastructure.Services;

public class CommunicationsService : ICommunicationsService
{
    private readonly IOptions<NotifyOptions> _options;
    private readonly ILogger<CommunicationsService> _logger;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Throttle window for "configuration is missing" warnings (blank API key, missing/malformed
    /// template) so that a sustained stream of send attempts (e.g. every login when 2FA is in
    /// use) produces a single structured log entry per distinct cause every
    /// <see cref="ConfigurationWarningThrottle"/>, instead of flooding the logs with one entry
    /// (or, previously, one full exception) per request.
    ///
    /// The throttle state is tracked per configuration reason (e.g. "missing-api-key",
    /// "missing-template:TwoFactorCode:SmsTemplateId") in <see cref="_lastConfigurationWarningLoggedAt"/>.
    /// This dictionary is bounded by the number of distinct template keys and id field names
    /// (Email/Sms), so it will not grow without limit - at most ~10 entries even with all
    /// templates fully configured.
    /// </summary>
    private static readonly TimeSpan ConfigurationWarningThrottle = TimeSpan.FromMinutes(5);
    private readonly Dictionary<string, DateTimeOffset> _lastConfigurationWarningLoggedAt = new();
    private readonly object _configurationWarningLock = new();

    /// <summary>
    /// Throttle window for runtime/API errors (NotifyClientException with 401/403, network 
    /// timeouts, malformed keys, etc.) so that repeated send failures during an outage or 
    /// configuration error produce log entries at a predictable rate instead of flooding the logs
    /// with one entry per request.
    /// 
    /// Separate from <see cref="ConfigurationWarningThrottle"/> because configuration errors 
    /// (missing API key, missing template) and runtime errors (auth failure, API unavailable) 
    /// are distinct problems requiring separate throttle buckets.
    ///
    /// Failures inside the window are not logged individually but are counted, and the count is
    /// reported on the next emitted entry (<c>SuppressedCount</c>) so the true failure volume
    /// remains visible to monitoring.
    /// </summary>
    private static readonly TimeSpan RuntimeErrorThrottle = TimeSpan.FromMinutes(5);
    private readonly Dictionary<string, (DateTimeOffset LastLoggedAt, int SuppressedCount)> _runtimeErrorState = new();
    private readonly object _runtimeErrorLock = new();

    /// <summary>
    /// Lazy-initialized GOV.UK Notify client. Built once at first send attempt, then reused.
    /// If construction fails (e.g. malformed API key), the exception is logged once during 
    /// initialization and never rethrown; subsequent sends will treat it as a configuration
    /// error and return Result.Failure(), keeping the client null and preventing an HttpClient 
    /// from being created per send.
    /// </summary>
    private readonly Lazy<NotificationClient?> _client;

    public CommunicationsService(
        IOptions<NotifyOptions> options,
        ILogger<CommunicationsService> logger,
        TimeProvider? timeProvider = null)
    {
        _options = options;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
        _client = new Lazy<NotificationClient?>(() => CreateClient());
    }

    public async Task<Result> SendSmsCodeAsync(string mobileNumber, string code)
    {
        if (TryGetClient(out var client, "GOV.UK Notify client is unavailable. SMS code not sent.") is false)
        {
            return Result.Failure("Unable to send verification code: messaging service is not configured.");
        }

        if (TryGetTemplateId("TwoFactorCode", t => t.SmsTemplateId, "SmsTemplateId", out var templateId) is false)
        {
            return Result.Failure("Unable to send verification code: messaging service is not configured.");
        }

        try
        {
            await client.SendSmsAsync(mobileNumber: mobileNumber,
            templateId: templateId,
            personalisation: new Dictionary<string, dynamic>()
            {
                {
                    "body", 
                    $"Your two factor authentication code is {code}"
                }
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            LogRuntimeErrorThrottled("sms-send", e, "Failed to send SMS code.");
            return Result.Failure("Unable to send verification code. Please try again or contact support.");
        }
    }
    
    public async Task<Result> SendEmailCodeAsync(string email, string code)
    {
        if (TryGetClient(out var client, "GOV.UK Notify client is unavailable. Email code not sent.") is false)
        {
            return Result.Failure("Unable to send verification code: messaging service is not configured.");
        }

        if (TryGetTemplateId("TwoFactorCode", t => t.EmailTemplateId, "EmailTemplateId", out var templateId) is false)
        {
            return Result.Failure("Unable to send verification code: messaging service is not configured.");
        }

        try
        {
            await client.SendEmailAsync(emailAddress: email,
            templateId: templateId,
            personalisation: new Dictionary<string, dynamic>() {
                {
                    "subject",
                    "Your two factor authentication code."
                },
                {
                    "body", $"Your two factor authentication code is {code}"
                }
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            LogRuntimeErrorThrottled("email-code", e, "Failed to send email code.");
            return Result.Failure("Unable to send verification code. Please try again or contact support.");
        }
    }
  
    public async Task<Result> SendAccountDeactivationEmail(string email)
    {
        if (TryGetClient(out var client, "GOV.UK Notify client is unavailable. Deactivation email not sent.") is false)
        {
            return Result.Failure("Unable to send account deactivation email: messaging service is not configured.");
        }

        if (TryGetTemplateId("AccountDeactivationReminder", t => t.EmailTemplateId, "EmailTemplateId", out var templateId) is false)
        {
            return Result.Failure("Unable to send account deactivation email: messaging service is not configured.");
        }

        try
        {
            await client.SendEmailAsync(emailAddress: email,
            templateId: templateId,
            personalisation: new Dictionary<string, dynamic>() {
                {
                    "subject",
                    "Your account will be deactivated."
                },
                {
                    "body", 
                    "Your account will be deactivated if you do not login soon"
                }
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            LogRuntimeErrorThrottled("account-deactivation", e, "Failed to send account deactivation email.");
            return Result.Failure("Unable to send account deactivation email. Please try again or contact support.");
        }
    }

    public async Task<Result> SendLoginThresholdAlertEmailAsync(string email, string subject, string body)
    {
        if (TryGetClient(out var client, "GOV.UK Notify client is unavailable. Login alert not sent.") is false)
        {
            return Result.Failure("Unable to send login threshold alert email: messaging service is not configured.");
        }

        if (TryGetTemplateId("LoginThresholdAlert", t => t.EmailTemplateId, "EmailTemplateId", out var templateId) is false)
        {
            return Result.Failure("Unable to send login threshold alert email: messaging service is not configured.");
        }

        try
        {
            await client.SendEmailAsync(emailAddress: email,
            templateId: templateId,
            personalisation: new Dictionary<string, dynamic>() {
                {
                    "subject",
                    subject
                },
                {
                    "body",
                    body
                }
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            LogRuntimeErrorThrottled("login-alert", e, "Failed to send login threshold alert email.");
            return Result.Failure("Unable to send login threshold alert email. Please try again or contact support.");
        }
    }

    /// <summary>
    /// Creates the GOV.UK Notify client once, at first send attempt, and caches the outcome for
    /// the lifetime of the process (this service is a singleton). A blank API key returns null
    /// silently - <see cref="NotifyConfigurationStartupCheck"/> already reports that at boot, and
    /// <see cref="TryGetClient"/> reports it (throttled) on each send attempt. A malformed key is
    /// logged as an error here, once. Correcting the key therefore requires an application restart.
    /// </summary>
    private NotificationClient? CreateClient()
    {
        if (string.IsNullOrWhiteSpace(_options.Value.ApiKey))
        {
            return null;
        }

        try
        {
            return new NotificationClient(_options.Value.ApiKey);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create GOV.UK Notify client. Notify:ApiKey is present but invalid or malformed. " +
                "Two-factor authentication codes and notification emails will not be sent.");
            return null;
        }
    }

    /// <summary>
    /// Attempts to get the GOV.UK Notify client. If the client is null (due to missing or invalid
    /// API key), logs a rate-limited warning and returns false. This ensures that missing/invalid
    /// configuration is visible in logs even in long-running pods, with one warning every 5 minutes.
    /// </summary>
    private bool TryGetClient([NotNullWhen(true)] out NotificationClient? client, string contextMessage)
    {
        client = _client.Value;
        if (client is null)
        {
            LogConfigurationWarningThrottled(
                "notify-client-unavailable",
                contextMessage);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Resolves a GOV.UK Notify template id (SMS or Email) for the given logical template key,
    /// without relying on null-forgiving operators. If the template is missing entirely, or the
    /// requested template id (SMS/Email) is not configured on it, this fails closed: logs a
    /// throttled, structured warning and returns <c>false</c> instead of letting a
    /// <see cref="NullReferenceException"/> bubble into the send path.
    /// </summary>
    private bool TryGetTemplateId(string templateKey, Func<Template, string?> selector, string idFieldName, out string templateId)
    {
        var template = _options.Value.GetTemplate(templateKey);
        var id = template is null ? null : selector(template);

        if (string.IsNullOrWhiteSpace(id))
        {
            LogConfigurationWarningThrottled(
                $"missing-template:{templateKey}:{idFieldName}",
                "Notify template configuration for '{TemplateKey}' is missing or has no {IdFieldName} configured. Unable to send this notification.",
                templateKey,
                idFieldName);
            templateId = string.Empty;
            return false;
        }

        templateId = id;
        return true;
    }

    /// <summary>
    /// Logs a single structured warning for a given configuration problem (identified by
    /// <paramref name="reasonKey"/>), throttled per-reason so that a burst of calls (e.g. many
    /// concurrent login attempts) does not flood the logs with duplicate entries. This is a
    /// deliberate "fail closed, log once" pattern rather than letting the underlying client throw
    /// per request.
    /// </summary>
    private void LogConfigurationWarningThrottled(string reasonKey, string message, params object?[] args)
    {
        lock (_configurationWarningLock)
        {
            var now = _timeProvider.GetUtcNow();
            if (_lastConfigurationWarningLoggedAt.TryGetValue(reasonKey, out var lastLoggedAt) &&
                now - lastLoggedAt < ConfigurationWarningThrottle)
            {
                return;
            }

            _lastConfigurationWarningLoggedAt[reasonKey] = now;
        }

        _logger.LogWarning(message + " This warning is throttled to once every {ThrottleMinutes} minute(s).",
            [.. args, ConfigurationWarningThrottle.TotalMinutes]);
    }

    /// <summary>
    /// Logs a single structured error (including the exception) for a runtime/API failure
    /// (identified by <paramref name="reasonKey"/>), throttled per-reason so that repeated API
    /// failures during an outage do not flood the logs. Failures suppressed inside the window are
    /// counted and reported as <c>SuppressedCount</c> on the next emitted entry, so the real
    /// failure volume is not hidden from monitoring.
    /// Uses LogError (not LogWarning) because these represent real 2FA/notification failures
    /// that should trigger alerts when monitoring on Error level.
    /// </summary>
    private void LogRuntimeErrorThrottled(string reasonKey, Exception exception, string message)
    {
        int suppressedCount;

        lock (_runtimeErrorLock)
        {
            var now = _timeProvider.GetUtcNow();
            var hasState = _runtimeErrorState.TryGetValue(reasonKey, out var state);

            if (hasState && now - state.LastLoggedAt < RuntimeErrorThrottle)
            {
                _runtimeErrorState[reasonKey] = (state.LastLoggedAt, state.SuppressedCount + 1);
                return;
            }

            suppressedCount = hasState ? state.SuppressedCount : 0;
            _runtimeErrorState[reasonKey] = (now, 0);
        }

        _logger.LogError(
            exception,
            message + " {SuppressedCount} similar failure(s) were suppressed since the previous entry. " +
            "This error is throttled to once every {ThrottleMinutes} minute(s).",
            suppressedCount,
            RuntimeErrorThrottle.TotalMinutes);
    }
}

public class NotifyOptions
{
    public const string Notify = "Notify";
    public required string ApiKey { get; set; }
    public IEnumerable<Template> Templates { get; set; } = [];
    public Template? GetTemplate(string key) => Templates.FirstOrDefault(template => template.Key == key);
}

public class Template
{
    public required string Key { get; set; }
    public string? EmailTemplateId { get; set; }
    public string? SmsTemplateId { get; set; }
}

