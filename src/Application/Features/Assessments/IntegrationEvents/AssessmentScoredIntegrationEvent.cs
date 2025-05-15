namespace Cfo.Cats.Application.Features.Assessments.IntegrationEvents;

public record AssessmentScoredIntegrationEvent(Guid Id, string ParticipantId, DateTime OccurredOn);