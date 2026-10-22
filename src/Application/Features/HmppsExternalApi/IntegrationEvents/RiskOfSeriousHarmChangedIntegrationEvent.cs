using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.HmppsExternalApi.IntegrationEvents;

/// <summary>
/// Internal integration event raised when HMPPS notifies us that a person's name has changed.
/// Published to the existing event bus (via the outbox) so the rest of the application can react
/// without any awareness of AWS, SQS or HMPPS-specific infrastructure.
/// </summary>
/// <param name="HmppsId">The HMPPS identifier of the person whose name changed.</param>
/// <param name="EventId">The originating HMPPS event identifier.</param>
/// <param name="EventType">The raw HMPPS event type.</param>
/// <param name="OccurredOn">When the event was processed.</param>
public record RiskOfSeriousHarmChangedIntegrationEvent(string HmppsId, long EventId, string EventType, DateTime OccurredOn)
    : IntegrationEvent(OccurredOn);
