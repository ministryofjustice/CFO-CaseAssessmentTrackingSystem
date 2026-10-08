using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Rebus.Bus;

namespace Cfo.Cats.Infrastructure.Services;

/// <summary>
/// Publishes <see cref="UsageTrackedIntegrationEvent"/> directly to the message bus,
/// deliberately bypassing the transactional outbox because the odd lost telemetry
/// message is acceptable and we never want to replay it.
/// </summary>
public class UsageTracker(IBus bus)
    : IUsageTracker
{
    public async Task TrackAsync(string area, string activity, UserProfile userProfile, string? context = null, CancellationToken cancellationToken = default)
    {
        var message = new UsageTrackedIntegrationEvent(
            Area: area,
            Activity: activity,
            UserId: userProfile.UserId,
            UserName: userProfile.UserName,
            TenantId: userProfile.TenantId,
            Context: context,
            OccurredOn: DateTime.UtcNow);

        await bus.Publish(message);
    }
}
