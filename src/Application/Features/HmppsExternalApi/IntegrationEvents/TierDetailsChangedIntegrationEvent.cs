using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.HmppsExternalApi.IntegrationEvents;

/// <summary>
/// Internal integration event raised when HMPPS notifies us that a person's tier details have changed.
/// </summary>
/// <param name="HmppsId">The HMPPS identifier of the person whose tier details changed.</param>
/// <param name="EventId">The originating HMPPS event identifier.</param>
/// <param name="EventType">The raw HMPPS event type.</param>
/// <param name="OccurredOn">When the event was processed.</param>
public record TierDetailsChangedIntegrationEvent(string HmppsId, long EventId, string EventType, DateTime OccurredOn)
    : IntegrationEvent(OccurredOn);
