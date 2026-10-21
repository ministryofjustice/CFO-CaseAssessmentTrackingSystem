using Cfo.Cats.Application.Features.HmppsExternalApi.IntegrationEvents;
using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents.Handlers;

public class TierDetailsChangedHandler(
    IHmppsEventDataRetriever retriever,
    IUnitOfWork unitOfWork,
    ILogger<TierDetailsChangedHandler> logger) : IHmppsEventHandler
{
    public const string TierDetailsChanged = "TIER_DETAILS_CHANGED";

    public string EventType => TierDetailsChanged;

    public async Task Handle(HmppsNotification notification, CancellationToken cancellationToken)
    {
        // The notification is only a trigger - retrieve the authoritative data from the supplied URL.
        var payload = await retriever.RetrieveAsync(notification.Url, cancellationToken);

        if (payload is null)
        {
            logger.LogWarning(
                "No data returned from HMPPS event URL {Url} for event {EventId}",
                notification.Url,
                notification.EventId);
            return;
        }

        var @event = new TierDetailsChangedIntegrationEvent(
            notification.HmppsId,
            notification.EventId,
            notification.EventType,
            DateTime.UtcNow);

        await unitOfWork.DbContext.InsertOutboxMessage(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Published {Event} for HMPPS person {HmppsId} (event {EventId})",
            nameof(TierDetailsChangedIntegrationEvent),
            notification.HmppsId,
            notification.EventId);
    }
}
