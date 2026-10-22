using Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Resolves the correct <see cref="IHmppsEventHandler"/> for a notification's event type and invokes
/// it. Unknown event types are ignored (logged) so the queue is not blocked by events we don't yet
/// handle.
/// </summary>
public class HmppsEventDispatcher(
    IEnumerable<IHmppsEventHandler> handlers,
    ILogger<HmppsEventDispatcher> logger) : IHmppsEventDispatcher
{
    public async Task Dispatch(HmppsNotification notification, CancellationToken cancellationToken)
    {
        var handler = handlers.FirstOrDefault(h =>
            string.Equals(h.EventType, notification.EventType, StringComparison.OrdinalIgnoreCase));

        if (handler is null)
        {
            logger.LogInformation(
                "No handler registered for HMPPS event type {EventType} (event {EventId}); ignoring",
                notification.EventType,
                notification.EventId);
            return;
        }

        await handler.Handle(notification, cancellationToken);
    }
}
