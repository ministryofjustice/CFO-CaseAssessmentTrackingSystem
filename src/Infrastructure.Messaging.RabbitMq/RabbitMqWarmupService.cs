using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Connects the publisher at host startup, off any request path, so the first publish from a Blazor
/// circuit finds a ready channel. A broker that is not yet available does not stop the host; the
/// publisher reconnects on first use.
/// </summary>
internal sealed class RabbitMqWarmupService(
    RabbitMqIntegrationEventPublisher publisher,
    ILogger<RabbitMqWarmupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await publisher.EnsureReadyAsync(stoppingToken);
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "RabbitMQ publisher warm-up failed; it will reconnect on first publish");
        }
    }
}
