namespace Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents;

/// <summary>
/// Handles a single type of HMPPS external event. Implementations retrieve the authoritative data
/// (via the notification's URL), map it to an internal integration event, and publish it using the
/// existing outbox / event bus. Implementations must not contain any AWS, SQS or certificate concerns.
/// </summary>
public interface IHmppsEventHandler
{
    /// <summary>
    /// The <see cref="HmppsNotification.EventType"/> value this handler is responsible for, e.g.
    /// "PERSON_NAME_CHANGED". Used by the dispatcher to route notifications.
    /// </summary>
    string EventType { get; }

    Task Handle(HmppsNotification notification, CancellationToken cancellationToken);
}
