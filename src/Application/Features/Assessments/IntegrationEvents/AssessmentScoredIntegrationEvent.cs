using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Assessments.IntegrationEvents;

public record AssessmentScoredIntegrationEvent(Guid AssessmentId, string ParticipantId, DateTime OccurredOn) : IntegrationEvent;