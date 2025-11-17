using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Inductions.IntegrationEvents;

public sealed record WingInductionCreatedIntegrationEvent(Guid InductionId, DateTime OccurredOn) : IntegrationEvent;

public sealed record HubInductionCreatedIntegrationEvent(Guid InductionId, DateTime OccurredOn) : IntegrationEvent;

public sealed record WingPhaseCompletedIntegrationEvent(Guid InductionId, int Phase): IntegrationEvent;