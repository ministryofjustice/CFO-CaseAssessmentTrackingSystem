namespace Cfo.Cats.Application.Features.Inductions.IntegrationEvents;

public record WingInductionCreatedIntegrationEvent(Guid Id);

public record HubInductionCreatedIntegrationEvent(Guid Id);

public record WingPhaseCompletedIntegrationEvent(Guid inductionId, int phase);