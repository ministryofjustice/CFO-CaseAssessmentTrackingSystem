using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ObjectiveTask : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ObjectiveTask()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public void Complete(string? justification = null)
    {
        Completed = DateTime.UtcNow;
        Justification = justification;
        CompletedStatus = TaskCompletionStatus.Done;
        //CompletedBy = "";
        AddDomainEvent(new ObjectiveTaskCompletedDomainEvent(this));
    }

    public void Close(string justification)
    {
        Justification = justification;
        Completed = DateTime.UtcNow;
        CompletedStatus = TaskCompletionStatus.Closed;
        //CompletedBy = "";
        //AddDomainEvent(new ObjectiveTaskCancelledDomainEvent(this));
    }

    public static ObjectiveTask Create(string title, DateTime due)
    {
        ObjectiveTask task = new()
        {
            Title = title,
            Due = due
        };

        task.AddDomainEvent(new ObjectiveTaskCreatedDomainEvent(task));
        return task;
    }

    public void Extend(DateTime due)
    {
        Due = due;
    }

    public void Rename(string title)
    {
        Title = title;
    }

    public DateTime Due { get; private set; }
    public DateTime? Completed { get; private set; }
    public string? CompletedBy { get; private set; }
    public TaskCompletionStatus? CompletedStatus { get; private set; }

    public string? Justification { get; private set; }
    public Guid ObjectiveId { get; private set; }
    public string Title { get; private set; }

    public virtual ApplicationUser? CompletedByUser { get; set; }
}
