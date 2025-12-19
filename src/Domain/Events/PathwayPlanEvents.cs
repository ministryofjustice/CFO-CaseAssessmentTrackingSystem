using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class PathwayPlanCreatedDomainEvent(PathwayPlan pathwayPlan) : DomainEvent
{
    public PathwayPlan Item { get; set; } = pathwayPlan;
}

public sealed class ObjectiveCreatedDomainEvent(Objective objective) : DomainEvent
{
    public Objective Item { get; set; } = objective;
}

public sealed class ObjectiveCompletedDomainEvent(Objective objective) : DomainEvent
{
    public Objective Item { get; set; } = objective;
}

public sealed class ObjectiveTaskAddedToObjectiveDomainEvent(Objective objective, ObjectiveTask task) : DomainEvent
{
    public Objective Item { get; set; } = objective;
    public ObjectiveTask Item2 { get; set; } = task;
}

public sealed class ObjectiveTaskCreatedDomainEvent(ObjectiveTask objectiveTask) : DomainEvent
{
    public ObjectiveTask Item { get; set; } = objectiveTask;
}

public sealed class ObjectiveTaskCompletedDomainEvent(ObjectiveTask objectiveTask) : DomainEvent
{
    public ObjectiveTask Item { get; set; } = objectiveTask;
}

public sealed class PathwayPlanReviewAddedDomainEvent(PathwayPlanReview pathwayPlanReview) : DomainEvent
{
    public PathwayPlanReview Item { get; set; } = pathwayPlanReview;
}
