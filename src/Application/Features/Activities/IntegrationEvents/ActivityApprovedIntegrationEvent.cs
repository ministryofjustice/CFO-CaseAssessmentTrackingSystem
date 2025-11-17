using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Activities.IntegrationEvents;

public record ActivityApprovedIntegrationEvent(Guid ActivityId, DateTime OccurredOn) : IntegrationEvent;
