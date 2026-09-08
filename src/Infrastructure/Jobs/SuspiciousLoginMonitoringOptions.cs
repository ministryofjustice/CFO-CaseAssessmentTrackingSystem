namespace Cfo.Cats.Infrastructure.Jobs;

/// <summary>
/// Configuration for <see cref="MonitorSuspiciousLoginActivityJob"/>.
/// Bound from the "SuspiciousLoginMonitoring" configuration section.
/// </summary>
public class SuspiciousLoginMonitoringOptions
{
    public const string Key = "SuspiciousLoginMonitoring";

    /// <summary>
    /// The number of failed login attempts from a single IP address (or against a
    /// single username) within <see cref="EvaluationWindowSeconds"/> that triggers
    /// an alert. Tune this to represent rules such as "200 attempts in 60 seconds".
    /// </summary>
    public int FailedAttemptThreshold { get; set; } = 50;

    /// <summary>
    /// The size of the rolling window, in seconds, over which failed attempts are counted.
    /// </summary>
    public int EvaluationWindowSeconds { get; set; } = 300;

    /// <summary>
    /// Usernames (or fragments of usernames) that should be treated as suspicious the
    /// moment they appear in a login attempt, e.g. "admin", "root", "administrator".
    /// Matching is case-insensitive and uses a "contains" comparison so that both
    /// "admin" and "admin@justice.gov.uk" are caught.
    /// </summary>
    public string[] MonitoredUserNames { get; set; } = [];

    /// <summary>
    /// Email addresses that should receive a GOV.UK Notify alert (using the
    /// "LoginThresholdAlert" template) when suspicious login activity is detected.
    /// A single digest email is sent to each recipient per scan that raises alerts.
    /// </summary>
    public string[] DistributionList { get; set; } = [];
}
