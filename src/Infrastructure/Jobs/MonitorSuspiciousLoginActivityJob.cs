using Microsoft.Extensions.Options;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;

/// <summary>
/// Periodically inspects the identity audit trail for signs of an attack against the
/// login system, such as a burst of failed attempts from a single IP address, repeated
/// attempts against a single account, or attempts against monitored/high-value usernames
/// (e.g. "admin@justice.gov.uk").
///
/// When suspicious activity is detected the job:
///  1. Writes a high severity structured log entry (surfaced via OpenTelemetry in the
///     Aspire dashboard / Grafana, where alert rules can be attached).
///  2. Records a <see cref="IdentityActionType.SuspiciousActivityDetected"/> entry in the
///     identity audit trail so it is visible to administrators in the UI.
/// </summary>
public class MonitorSuspiciousLoginActivityJob(
    ILogger<MonitorSuspiciousLoginActivityJob> logger,
    IUnitOfWork unitOfWork,
    IOptions<SuspiciousLoginMonitoringOptions> options) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(MonitorSuspiciousLoginActivityJob));
    public static readonly string Description = "A job to monitor and alert on suspicious login activity (brute force / username guessing).";

    private const string PerformedBy = "Suspicious Login Monitor";
    private const string MultipleMarker = "(multiple)";

    private static readonly IdentityActionType[] FailedActionTypes =
    [
        IdentityActionType.UnknownUser,
        IdentityActionType.IncorrectPasswordEntered,
        IdentityActionType.IncorrectTwoFactorCodeEntered,
        IdentityActionType.UserAccountLockedOut
    ];

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["JobName"] = Key.Name,
            ["JobGroup"] = Key.Group ?? "Default",
            ["JobInstance"] = Guid.NewGuid().ToString()
        }))
        {
            if (context.RefireCount > 3)
            {
                logger.LogWarning("Failed to complete Monitor Suspicious Login Activity Job within 3 tries, aborting...");
                return;
            }
        }

        try
        {
            var settings = options.Value;
            var threshold = Math.Max(1, settings.FailedAttemptThreshold);
            var windowSeconds = Math.Max(1, settings.EvaluationWindowSeconds);
            var windowStart = DateTime.Now.AddSeconds(-windowSeconds);
            var cancellationToken = context.CancellationToken;

            logger.LogInformation(
                "Starting suspicious login activity check. Threshold={Threshold} failed attempts within {WindowSeconds}s window",
                threshold, windowSeconds);

            // Existing alerts within the window, used to avoid raising duplicate alerts
            // on every scheduled run while an attack is still ongoing.
            var existingAlerts = await unitOfWork.DbContext.IdentityAuditTrails
                .Where(a => a.ActionType == IdentityActionType.SuspiciousActivityDetected && a.DateTime >= windowStart)
                .Select(a => new { a.IpAddress, a.UserName })
                .ToListAsync(cancellationToken);

            var failedAttempts = unitOfWork.DbContext.IdentityAuditTrails
                .Where(a => FailedActionTypes.Contains(a.ActionType) && a.DateTime >= windowStart);

            var newAlerts = new List<IdentityAuditTrail>();

            // Rule A: a single IP address making many failed attempts (automated attack).
            var offendingIps = await failedAttempts
                .Where(a => a.IpAddress != null)
                .GroupBy(a => a.IpAddress)
                .Select(g => new { IpAddress = g.Key!, Count = g.Count() })
                .Where(x => x.Count >= threshold)
                .ToListAsync(cancellationToken);

            foreach (var offender in offendingIps)
            {
                if (existingAlerts.Any(a => a.IpAddress == offender.IpAddress && a.UserName == null))
                {
                    continue;
                }

                logger.LogWarning(
                    "SECURITY ALERT: Suspicious login activity detected. Rule={Rule} IpAddress={IpAddress} FailedAttempts={Count} WindowSeconds={WindowSeconds}",
                    "HighVolumeFromIpAddress", offender.IpAddress, offender.Count, windowSeconds);

                newAlerts.Add(IdentityAuditTrail.Create(null, PerformedBy, IdentityActionType.SuspiciousActivityDetected, offender.IpAddress));
            }

            // Rule B: a single account being targeted by many failed attempts (credential stuffing).
            var targetedUsers = await failedAttempts
                .Where(a => a.UserName != null)
                .GroupBy(a => a.UserName)
                .Select(g => new { UserName = g.Key!, Count = g.Count() })
                .Where(x => x.Count >= threshold)
                .ToListAsync(cancellationToken);

            foreach (var target in targetedUsers)
            {
                if (existingAlerts.Any(a => a.UserName == target.UserName))
                {
                    continue;
                }

                logger.LogWarning(
                    "SECURITY ALERT: Suspicious login activity detected. Rule={Rule} UserName={UserName} FailedAttempts={Count} WindowSeconds={WindowSeconds}",
                    "HighVolumeAgainstUser", target.UserName, target.Count, windowSeconds);

                newAlerts.Add(IdentityAuditTrail.Create(target.UserName, PerformedBy, IdentityActionType.SuspiciousActivityDetected, MultipleMarker));
            }

            // Rule C: any attempt against a monitored/high-value username (e.g. admin@...).
            var monitoredUserNames = (settings.MonitoredUserNames ?? [])
                .Where(u => string.IsNullOrWhiteSpace(u) == false)
                .Select(u => u.Trim().ToLowerInvariant())
                .Distinct()
                .ToArray();

            if (monitoredUserNames.Length > 0)
            {
                var attemptedUserNames = await failedAttempts
                    .Where(a => a.UserName != null)
                    .Select(a => a.UserName!)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                foreach (var keyword in monitoredUserNames)
                {
                    var matches = attemptedUserNames
                        .Where(u => u.ToLowerInvariant().Contains(keyword))
                        .ToList();

                    foreach (var matchedUserName in matches)
                    {
                        // Skip if already alerted on this username via any rule in the window.
                        if (existingAlerts.Any(a => a.UserName == matchedUserName)
                            || newAlerts.Any(a => a.UserName == matchedUserName))
                        {
                            continue;
                        }

                        logger.LogWarning(
                            "SECURITY ALERT: Suspicious login activity detected. Rule={Rule} UserName={UserName} MatchedKeyword={Keyword} WindowSeconds={WindowSeconds}",
                            "MonitoredUserName", matchedUserName, keyword, windowSeconds);

                        newAlerts.Add(IdentityAuditTrail.Create(matchedUserName, PerformedBy, IdentityActionType.SuspiciousActivityDetected, MultipleMarker));
                    }
                }
            }

            if (newAlerts.Count > 0)
            {
                await unitOfWork.DbContext.IdentityAuditTrails.AddRangeAsync(newAlerts, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogWarning("Raised {AlertCount} new suspicious login activity alert(s)", newAlerts.Count);
            }
            else
            {
                logger.LogInformation("No suspicious login activity detected");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Quartz job {Key} failed", Key.Name);
            throw new JobExecutionException(msg: "An unexpected error occurred executing Monitor Suspicious Login Activity job", refireImmediately: true, cause: ex);
        }
    }
}
