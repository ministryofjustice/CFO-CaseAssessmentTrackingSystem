using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Inductions.IntegrationEvents;

public record WingInductionCreatedIntegrationEvent(Guid Id, DateTime OccurredOn) : IntegrationEvent(OccurredOn);

public record HubInductionCreatedIntegrationEvent(Guid Id, DateTime OccurredOn)  : IntegrationEvent(OccurredOn);

public record WingPhaseCompletedIntegrationEvent(Guid InductionId, int Phase, DateTime OccurredOn) : IntegrationEvent(OccurredOn);