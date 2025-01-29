namespace Cfo.Cats.Application.Features.PathwayPlans.IntegrationEvents;

public record ObjectiveTaskCompletedIntegrationEvent(
    Guid TaskId,
    Guid ObjectiveId,
    bool IsMandatoryTask,
    int Index,
    string CompletionState);
