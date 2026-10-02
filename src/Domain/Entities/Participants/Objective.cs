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
        Id = Guid.CreateVersion7();
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

    /// <summary>
    /// Reason given for removing or replacing the linked initiative — recorded here, on the objective
    /// itself, because the InitiativeObjective row being removed/replaced does not survive the change
    /// (it's either deleted outright on unlink, or has its InitiativeId overwritten in place on a swap).
    /// Deliberately separate from <see cref="Justification"/>, which is reserved for objective
    /// completion, so that completing an objective can never overwrite why its initiative was previously
    /// changed, or vice versa. Only reflects the most recent justification given; the full history of
    /// changes to this value is retained in the system audit trail (AuditTrail).
    /// </summary>
    public string? InitiativeChangeJustification { get; private set; }

    public InitiativeObjective? LinkedInitiative { get; private set; }

    public IReadOnlyCollection<ObjectiveTask> Tasks => _tasks.AsReadOnly();

    public bool IsCompleted => Completed is not null;

    public bool IsMandatory { get; private set; }

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

    public void Rename(string description) => Description = description;

    public void RecordInitiativeChangeJustification(string? justification) => InitiativeChangeJustification = justification;

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

    public static Objective Create(string description, Guid pathwayPlanId, bool isMandatory = false)
    {
        Objective objective = new()
        {
            Description = description,
            PathwayPlanId = pathwayPlanId,
            IsMandatory = isMandatory
        };

        objective.AddDomainEvent(new ObjectiveCreatedDomainEvent(objective));
        return objective;
    }

    public virtual ApplicationUser? CompletedByUser { get; set; }
    public virtual ApplicationUser? CreatedByUser { get; set; }

}
