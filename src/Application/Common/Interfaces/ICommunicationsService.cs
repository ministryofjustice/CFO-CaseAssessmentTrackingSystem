namespace Cfo.Cats.Application.Common.Interfaces;

public interface ICommunicationsService
{
    /// <summary>
    /// Sends the account deactivation reminder email.
    /// Returns <see cref="Result.Failure(string[])"/> (rather than throwing) when the
    /// message could not be sent, e.g. because GOV.UK Notify or its templates are not configured.
    /// Callers should observe the result (e.g. log a warning) rather than discard it, so
    /// configuration problems remain visible to monitoring instead of being silently swallowed.
    /// </summary>
    Task<Result> SendAccountDeactivationEmail(string email);

    /// <summary>
    /// Sends a two-factor authentication code via SMS.
    /// Returns <see cref="Result.Failure(string[])"/> (rather than throwing) when the
    /// message could not be sent, e.g. because GOV.UK Notify is not configured.
    /// </summary>
    Task<Result> SendSmsCodeAsync(string mobileNumber, string code);

    /// <summary>
    /// Sends a two-factor authentication code via email.
    /// Returns <see cref="Result.Failure(string[])"/> (rather than throwing) when the
    /// message could not be sent, e.g. because GOV.UK Notify is not configured.
    /// </summary>
    Task<Result> SendEmailCodeAsync(string email, string code);

    /// <summary>
    /// Sends a suspicious login activity alert email.
    /// Returns <see cref="Result.Failure(string[])"/> (rather than throwing) when the
    /// message could not be sent, e.g. because GOV.UK Notify or its templates are not configured.
    /// Callers should observe the result (e.g. log a warning) rather than discard it, so
    /// configuration problems remain visible to monitoring instead of being silently swallowed.
    /// </summary>
    Task<Result> SendLoginThresholdAlertEmailAsync(string email, string subject, string body);
}
