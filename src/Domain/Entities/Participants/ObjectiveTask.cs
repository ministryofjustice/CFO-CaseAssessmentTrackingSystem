﻿using Cfo.Cats.Domain.Common.Entities;
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

    public void Complete(DateTime? nextDue)
    {
        Completed = DateTime.UtcNow;
        Due = nextDue;
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

    public DateTime? Due { get; private set; }
    public DateTime? Completed { get; set; }
    public string? CompletedBy { get; set; }
    public string Title { get; private set; }

    public virtual ApplicationUser? CompletedByUser { get; set; }

    // public bool IsOverdue() => Due > DateTime.UtcNow;
    // public bool IsDueSoon() => Due > DateTime.UtcNow.AddDays(-14);
}