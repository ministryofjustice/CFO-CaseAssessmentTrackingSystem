using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Domain.Events.QA.Payables;

public sealed class ActivityQa2EntryEscalatedDomainEvent(ActivityQa2QueueEntry entry) : DomainEvent
{
    public ActivityQa2QueueEntry Entry { get; } = entry;
}