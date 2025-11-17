using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.PRIs.IntegrationEvents;

public sealed record PRIAssignedIntegrationEvent(Guid PRIId, DateTime OccurredOn) : IntegrationEvent;

public sealed record PRIThroughTheGateCompletedIntegrationEvent(Guid PRIId) : IntegrationEvent;