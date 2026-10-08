using Microsoft.Extensions.Hosting;

namespace Cfo.Cats.Infrastructure.Services;

/// <summary>
/// Emits a single, clear, structured warning at application startup if GOV.UK Notify
/// (<see cref="NotifyOptions"/>) is not configured with an API key.
///
/// This hosted service is registered in <c>AddServices()</c>, which is called by:
/// - <c>AddInfrastructure()</c> (Server.UI / Blazor)
/// - <c>AddWorkerInfrastructure()</c> (Worker background service)
/// - <c>AddConsumerInfrastructure()</c> (Cats.Consumers)
///
/// So this startup check will fire once in each of these three host processes.
///
/// This is a deliberate "fail closed, signal once" safety net: a blank/missing
/// <c>Notify:ApiKey</c> must never throw or crash the application (two-factor authentication
/// and other notifications simply won't be delivered - see <see cref="CommunicationsService"/>),
/// but operators should get an unmissable, single signal at boot time in each host rather than
/// discovering the problem only after users start reporting they never received a 2FA code.
/// </summary>
public sealed class NotifyConfigurationStartupCheck(
    IOptions<NotifyOptions> options,
    ILogger<NotifyConfigurationStartupCheck> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
        {
            logger.LogWarning(
                "Startup check: Notify:ApiKey is not configured. Two-factor authentication codes, " +
                "account deactivation reminders and login threshold alerts will NOT be delivered until " +
                "this is configured. This will not cause errors or crashes - affected sends will fail " +
                "closed and be logged (throttled) at runtime.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}