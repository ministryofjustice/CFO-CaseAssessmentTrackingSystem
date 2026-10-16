using Cfo.Cats.Application.Features.HmppsExternalApi.IntegrationEvents;
using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents.Handlers;

public class RiskOfSeriousHarmChangedHandler(
    IHmppsEventDataRetriever retriever,
    IUnitOfWork unitOfWork,
    ILogger<RiskOfSeriousHarmChangedHandler> logger) : IHmppsEventHandler
{
    public const string RiskOfSeriousHarmChanged = "RISK_OF_SERIOUS_HARM_CHANGED";

    public string EventType => RiskOfSeriousHarmChanged;

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

        // TODO: map the retrieved payload to the internal domain once the contract is defined.
        var @event = new RiskOfSeriousHarmChangedIntegrationEvent(
            notification.HmppsId,
            notification.EventId,
            DateTime.UtcNow);

        await unitOfWork.DbContext.InsertOutboxMessage(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Published {Event} for HMPPS person {HmppsId} (event {EventId})",
            nameof(RiskOfSeriousHarmChangedIntegrationEvent),
            notification.HmppsId,
            notification.EventId);
    }
}
