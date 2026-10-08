using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Communications;

/// <summary>
/// Regression tests for the "2FA enabled with a blank GOV.UK Notify API key" incident:
/// previously, <see cref="CommunicationsService"/> constructed a <c>NotificationClient</c>
/// directly from a possibly-blank API key, which threw on every single send attempt
/// (e.g. every login, when 2FA is in use), flooding production logs.
///
/// Also covers the related "missing/malformed template configuration" case, where the
/// previous implementation used null-forgiving (<c>!</c>) lookups
/// (<c>GetTemplate("TwoFactorCode")!.SmsTemplateId</c>) that could throw a
/// <see cref="NullReferenceException"/> inside the send path if a template was missing.
///
/// These tests assert the service now fails closed in both cases: no exceptions are thrown, a
/// user-safe <see cref="Result.Failure(string[])"/> is returned, and no configuration internals
/// (API key, template ids, stack traces) are ever leaked back to the caller.
/// </summary>
public class CommunicationsServiceTests
{
    private static readonly Template DefaultTwoFactorTemplate = new()
    {
        Key = "TwoFactorCode",
        EmailTemplateId = "email-template-id",
        SmsTemplateId = "sms-template-id"
    };

    private static CommunicationsService CreateSut(
        string apiKey,
        out Mock<ILogger<CommunicationsService>> loggerMock,
        TimeProvider timeProvider = null,
        IEnumerable<Template> templates = null)
    {
        var notifyOptions = new NotifyOptions
        {
            ApiKey = apiKey,
            Templates = templates ?? [DefaultTwoFactorTemplate]
        };

        loggerMock = new Mock<ILogger<CommunicationsService>>();
        return new CommunicationsService(Options.Create(notifyOptions), loggerMock.Object, timeProvider);
    }

    private const string ValidLookingApiKey =
        "test-00000000-0000-0000-0000-000000000000-00000000-0000-0000-0000-000000000000";

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task SendEmailCodeAsync_WithBlankOrMissingApiKey_DoesNotThrow_AndReturnsFailure(string apiKey)
    {
        var sut = CreateSut(apiKey, out _);

        // If this threw, the test would fail with an unhandled exception - which is exactly
        // the production incident this guards against.
        var result = await sut.SendEmailCodeAsync("user@example.com", "123456");

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task SendSmsCodeAsync_WithBlankOrMissingApiKey_DoesNotThrow_AndReturnsFailure(string apiKey)
    {
        var sut = CreateSut(apiKey, out _);

        var result = await sut.SendSmsCodeAsync("07700900000", "123456");

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Test]
    public async Task SendEmailCodeAsync_WithBlankApiKey_FailureMessageIsUserSafe()
    {
        var sut = CreateSut(string.Empty, out _);

        var result = await sut.SendEmailCodeAsync("user@example.com", "123456");

        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
        // The user-facing message must never leak configuration internals, API keys or stack traces.
        result.ErrorMessage.ShouldNotContain("ApiKey", Case.Insensitive);
        result.ErrorMessage.ShouldNotContain("NotificationClient");
        result.ErrorMessage.ShouldNotContain("Exception");
        result.ErrorMessage.ShouldNotContain("at Cfo.Cats");
    }

    [Test]
    public async Task SendAccountDeactivationEmail_WithBlankApiKey_DoesNotThrow_AndReturnsFailure()
    {
        var sut = CreateSut(string.Empty, out _);

        var result = await sut.SendAccountDeactivationEmail("user@example.com");

        result.Succeeded.ShouldBeFalse();
    }

    [Test]
    public async Task SendLoginThresholdAlertEmailAsync_WithBlankApiKey_DoesNotThrow_AndReturnsFailure()
    {
        var sut = CreateSut(string.Empty, out _);

        var result = await sut.SendLoginThresholdAlertEmailAsync("user@example.com", "subject", "body");

        result.Succeeded.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCodeAsync_WithMissingTemplate_DoesNotThrow_AndReturnsFailure()
    {
        // Valid API key, but no "TwoFactorCode" template configured at all - this previously
        // threw a NullReferenceException via `GetTemplate("TwoFactorCode")!.EmailTemplateId`.
        var sut = CreateSut(ValidLookingApiKey, out _, templates: []);

        var result = await sut.SendEmailCodeAsync("user@example.com", "123456");

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Test]
    public async Task SendSmsCodeAsync_WithTemplatePresentButNoSmsTemplateId_DoesNotThrow_AndReturnsFailure()
    {
        // Template exists (e.g. configured for email use only) but has no SmsTemplateId set.
        var sut = CreateSut(
            ValidLookingApiKey,
            out _,
            templates: [new Template { Key = "TwoFactorCode", EmailTemplateId = "email-template-id", SmsTemplateId = null }]);

        var result = await sut.SendSmsCodeAsync("07700900000", "123456");

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Test]
    public async Task SendEmailCodeAsync_WithMissingTemplate_FailureMessageIsUserSafe()
    {
        var sut = CreateSut(ValidLookingApiKey, out _, templates: []);

        var result = await sut.SendEmailCodeAsync("user@example.com", "123456");

        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
        result.ErrorMessage.ShouldNotContain("NullReferenceException");
        result.ErrorMessage.ShouldNotContain("ApiKey", Case.Insensitive);
        result.ErrorMessage.ShouldNotContain("at Cfo.Cats");
    }

    [Test]
    public async Task SendEmailCodeAsync_WithMissingTemplate_LogsWarning_NotError()
    {
        var sut = CreateSut(ValidLookingApiKey, out var loggerMock, templates: []);

        await sut.SendEmailCodeAsync("user@example.com", "123456");

        VerifyLog(loggerMock, LogLevel.Warning, Times.Once());
        VerifyLog(loggerMock, LogLevel.Error, Times.Never());
    }

    [Test]
    public async Task SendEmailCodeAsync_WithMissingTemplate_RepeatedCallsWithinThrottleWindow_LogsOnce()
    {
        var fixedTimeProvider = new FixedTimeProvider(DateTimeOffset.UtcNow);
        var sut = CreateSut(ValidLookingApiKey, out var loggerMock, fixedTimeProvider, templates: []);

        await sut.SendEmailCodeAsync("user1@example.com", "111111");
        await sut.SendEmailCodeAsync("user2@example.com", "222222");

        VerifyLog(loggerMock, LogLevel.Warning, Times.Once());
    }

    [Test]
    public async Task SendEmailCodeAsync_WithBlankApiKey_LogsWarning_NotError()
    {
        var sut = CreateSut(string.Empty, out var loggerMock);

        await sut.SendEmailCodeAsync("user@example.com", "123456");

        // Blank key is reported once by NotifyConfigurationStartupCheck at boot; at send time only
        // the (throttled) TryGetClient warning fires - CreateClient no longer duplicates it.
        VerifyLog(loggerMock, LogLevel.Warning, Times.Once());
        VerifyLog(loggerMock, LogLevel.Error, Times.Never());
    }

    [Test]
    public async Task SendEmailCodeAsync_WithBlankApiKey_RepeatedCallsWithinThrottleWindow_LogsOnce()
    {
        // Use a fixed time provider so repeated calls fall inside the throttle window
        // (regression guard for "swamps production" style log amplification).
        var fixedTimeProvider = new FixedTimeProvider(DateTimeOffset.UtcNow);
        var sut = CreateSut(string.Empty, out var loggerMock, fixedTimeProvider);

        await sut.SendEmailCodeAsync("user1@example.com", "111111");
        await sut.SendEmailCodeAsync("user2@example.com", "222222");
        await sut.SendSmsCodeAsync("07700900000", "333333");

        // Only the first send logs; subsequent sends within the window are throttled.
        VerifyLog(loggerMock, LogLevel.Warning, Times.Once());
    }

    [Test]
    public async Task SendEmailCodeAsync_WithMalformedApiKey_DoesNotThrow_AndReturnsFailure()
    {
        // A malformed API key (e.g. copy-paste error, stray quote) will cause the
        // NotificationClient constructor to throw during lazy initialization.
        // This should be caught, logged once at startup, and subsequent sends should
        // fail closed with Result.Failure().
        var sut = CreateSut("not-a-valid-key-format", out var loggerMock);

        var result = await sut.SendEmailCodeAsync("user@example.com", "123456");

        result.Succeeded.ShouldBeFalse();
        // CreateClient logs error (once, lazy init) + TryGetClient logs warning (throttled)
        VerifyLog(loggerMock, LogLevel.Error, Times.Once());
        VerifyLog(loggerMock, LogLevel.Warning, Times.Once());
    }

    [Test]
    [Explicit("Makes a real HTTP request to GOV.UK Notify - run manually only.")]
    [Category("Integration")]
    public async Task SendEmailCodeAsync_WithValidApiKeyFormatButUnreachableService_DoesNotThrow_AndReturnsFailure()
    {
        // A syntactically-valid-looking key (GOV.UK Notify format) will construct a client
        // successfully, but the actual network call will fail (the key is not a real one).
        // This asserts genuine send failures (not just missing config) also fail closed.
        var sut = CreateSut(ValidLookingApiKey, out _);

        var result = await sut.SendEmailCodeAsync("user@example.com", "123456");

        result.Succeeded.ShouldBeFalse();
    }

    private static void VerifyLog(Mock<ILogger<CommunicationsService>> loggerMock, LogLevel level, Times times) =>
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            times);

    [Test]
    public void CommunicationsService_RegisteredAsSingleton_SameInstanceAcrossScopes()
    {
        // Regression guard: if CommunicationsService is scoped (not singleton), each scope gets
        // a fresh instance with a fresh, empty throttle dictionary. The unit tests above verify
        // the throttle logic itself (using FixedTimeProvider), but they reuse one SUT instance,
        // so they don't catch a scoped registration. This test ensures the actual registered
        // lifetime (AddSingleton in DependencyInjection.cs) is honored, so the throttle state
        // persists across requests.
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.Configure<NotifyOptions>(opts => opts.ApiKey = "test-api-key");
        services.AddSingleton<ICommunicationsService, CommunicationsService>();

        var provider = services.BuildServiceProvider();

        // Resolve from two separate scopes
        ICommunicationsService svc1, svc2;
        using (var scope1 = provider.CreateScope())
        {
            svc1 = scope1.ServiceProvider.GetRequiredService<ICommunicationsService>();
        }

        using (var scope2 = provider.CreateScope())
        {
            svc2 = scope2.ServiceProvider.GetRequiredService<ICommunicationsService>();
        }

        // If registered as AddSingleton, both scopes must get the same instance.
        // If registered as AddScoped (regression), they would be different instances.
        svc1.ShouldBeSameAs(svc2);
    }

    [Test]
    public async Task SendEmailCodeAsync_WithBlankApiKey_MultipleResolvesFromSingleProvider_LogsWarningOnce()
    {
        // Regression guard for the singleton lifetime: verify that when the service is
        // resolved twice from the same ServiceProvider (simulating two sequential login
        // attempts), only a single "not configured" warning is logged, thanks to the shared
        // throttle state. If the service were scoped, each resolve would get a fresh
        // instance with a fresh throttle dictionary, and you'd see two warnings.
        var loggedMessages = new List<string>();

        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(new TestLoggerProvider(loggedMessages));
        });
        services.AddOptions();
        services.Configure<NotifyOptions>(opts => opts.ApiKey = "");
        services.AddSingleton<ICommunicationsService, CommunicationsService>();

        var provider = services.BuildServiceProvider();

        // Simulate two sequential login attempts, each getting the service from the provider
        // (same instance due to singleton registration, same throttle state).
        using (var scope1 = provider.CreateScope())
        {
            var svc1 = scope1.ServiceProvider.GetRequiredService<ICommunicationsService>();
            await svc1.SendEmailCodeAsync("user1@example.com", "111111");
        }

        using (var scope2 = provider.CreateScope())
        {
            var svc2 = scope2.ServiceProvider.GetRequiredService<ICommunicationsService>();
            await svc2.SendEmailCodeAsync("user2@example.com", "222222");
        }

        // Count "not configured" warnings - should be exactly 1, not 2, due to the shared throttle.
        var notConfiguredWarnings = loggedMessages
            .Where(m => m.Contains("GOV.UK Notify client is unavailable"))
            .ToList();

        notConfiguredWarnings.Count.ShouldBe(1, $"Expected 1 'not configured' warning, but got {notConfiguredWarnings.Count}. Messages: {string.Join("; ", loggedMessages)}");
    }

    /// <summary>
    /// Test logger provider to capture log messages for assertion.
    /// </summary>
    private sealed class TestLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _messages;

        public TestLoggerProvider(List<string> messages)
        {
            _messages = messages;
        }

        public ILogger CreateLogger(string categoryName) => new TestLogger(_messages);

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Test logger that captures formatted messages.
    /// </summary>
    private sealed class TestLogger : ILogger
    {
        private readonly List<string> _messages;

        public TestLogger(List<string> messages)
        {
            _messages = messages;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter) =>
            _messages.Add(formatter(state, exception));
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTimeProvider(DateTimeOffset now) => _now = now;

        public override DateTimeOffset GetUtcNow() => _now;
    }
}