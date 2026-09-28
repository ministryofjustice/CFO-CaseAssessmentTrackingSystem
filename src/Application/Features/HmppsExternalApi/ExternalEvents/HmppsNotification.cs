namespace Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents;

/// <summary>
/// The lightweight notification HMPPS publishes to its SNS topic (and hence our SQS queue) when
/// something changes about a person. It is only a trigger: the authoritative data must be retrieved
/// from <see cref="Url"/> rather than being trusted from the notification itself.
/// </summary>
public sealed class HmppsNotification
{
    /// <summary>
    /// The unique identifier HMPPS assigns to this event. Used for idempotency / de-duplication.
    /// </summary>
    public long EventId { get; init; }

    /// <summary>
    /// The HMPPS identifier of the person the event relates to.
    /// </summary>
    public string HmppsId { get; init; } = string.Empty;

    /// <summary>
    /// The type of event, e.g. "PERSON_NAME_CHANGED". Used to route to the correct handler.
    /// </summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// The URL from which the authoritative event data should be retrieved.
    /// </summary>
    public string Url { get; init; } = string.Empty;
}
