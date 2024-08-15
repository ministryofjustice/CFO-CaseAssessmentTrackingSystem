using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Objective : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Objective()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private List<ObjectiveTask> _tasks = new();

    public DateTime? Completed { get; set; }
    public CompletionStatus? CompletedStatus { get; set; }

    public string ParticipantId { get; private set; }

    public string Title { get; private set; }
    
    public string? Justification { get; private set; }

    public IReadOnlyCollection<ObjectiveTask> Tasks => _tasks.AsReadOnly();

    public Objective AddTask(ObjectiveTask task)
    {
        _tasks.Add(task);
        AddDomainEvent(new ObjectiveTaskAddedToObjectiveDomainEvent(this, task));
        return this;
    }

    public void Rename(string title)
    {
        Title = title;
    }

    public void Review(CompletionStatus status, string? justification)
    {
        foreach (var task in _tasks.Where(task => task.Completed is null))
        {
            task.Review(status, justification);
        }

        CompletedStatus = status;
        Completed = DateTime.UtcNow;
        Justification = justification;
        // AddDomainEvent
    }

    public static Objective Create(string title, string participantId)
    {
        Objective objective = new()
        {
            Title = title,
            ParticipantId = participantId
        };

        objective.AddDomainEvent(new ObjectiveCreatedDomainEvent(objective));
        return objective;
    }

}
