using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityQa2EntryCompletedDomainEvent(ActivityQa2QueueEntry entry) : DomainEvent
    {
        public ActivityQa2QueueEntry Entry { get; } = entry;
    }
}