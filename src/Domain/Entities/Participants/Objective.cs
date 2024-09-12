using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Objective : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Objective()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private List<ObjectiveTask> _tasks = new();

    public DateTime? Completed { get; private set; }
    public string? CompletedBy { get; private set; }
    public CompletionStatus? CompletedStatus { get; private set; }

    public int Index { get; private set; }

    public Guid PathwayPlanId { get; private set; }

    public string Description { get; private set; }
    
    public string? Justification { get; private set; }

    public IReadOnlyCollection<ObjectiveTask> Tasks => _tasks.AsReadOnly();

    public bool IsCompleted => Completed is not null;

    public Objective AddTask(ObjectiveTask task)
    {
        _tasks.Add(task.AtIndex(_tasks.Count + 1));
        AddDomainEvent(new ObjectiveTaskAddedToObjectiveDomainEvent(this, task));
        return this;
    }

    public Objective AtIndex(int index)
    {
        Index = index;
        return this;
    }

    public void Rename(string description)
    {
        Description = description;
    }

    public void Complete(CompletionStatus status, string completedBy, string? justification)
    {
        foreach (var task in _tasks.Where(task => task.IsCompleted is false))
        {
            task.Complete(status, completedBy, justification);
        }

        CompletedStatus = status;
        Completed = DateTime.UtcNow;
        CompletedBy = completedBy;
        Justification = justification;
        AddDomainEvent(new ObjectiveCompletedDomainEvent(this));
    }

    public static Objective Create(string description, Guid pathwayPlanId)
    {
        Objective objective = new()
        {
            Description = description,
            PathwayPlanId = pathwayPlanId
        };

        objective.AddDomainEvent(new ObjectiveCreatedDomainEvent(objective));
        return objective;
    }

    public virtual ApplicationUser? CompletedByUser { get; set; }

}
