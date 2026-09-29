using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Inductions.IntegrationEvents;

public record WingInductionCreatedIntegrationEvent(Guid WingInductionId, DateTime OccurredOn) : IntegrationEvent(OccurredOn);

public record HubInductionCreatedIntegrationEvent(Guid HubInductionId, DateTime OccurredOn)  : IntegrationEvent(OccurredOn);

public record WingPhaseCompletedIntegrationEvent(Guid WingInductionId, int Phase, DateTime OccurredOn) : IntegrationEvent(OccurredOn);