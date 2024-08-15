using Cfo.Cats.Domain.Common.Entities;
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

    public string ParticipantId { get; private set; }

    public string Title { get; private set; }

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
