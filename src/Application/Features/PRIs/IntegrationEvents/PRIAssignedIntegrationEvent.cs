using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.PRIs.IntegrationEvents;

public record PRIAssignedIntegrationEvent(Guid PRIId, DateTime OccurredOn) : IntegrationEvent(OccurredOn);

public record PRIThroughTheGateCompletedIntegrationEvent(Guid PRIId, DateTime OccurredOn) : IntegrationEvent(OccurredOn);