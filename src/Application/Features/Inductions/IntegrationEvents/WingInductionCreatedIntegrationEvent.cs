namespace Cfo.Cats.Application.Features.Inductions.IntegrationEvents;

public record WingInductionCreatedIntegrationEvent(Guid Id, DateTime OccurredOn);

public record HubInductionCreatedIntegrationEvent(Guid Id, DateTime OccurredOn);

public record WingPhaseCompletedIntegrationEvent(Guid InductionId, int Phase);