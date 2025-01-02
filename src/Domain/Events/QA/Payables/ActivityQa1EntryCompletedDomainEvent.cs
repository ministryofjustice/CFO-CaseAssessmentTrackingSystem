using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityQa1EntryCompletedDomainEvent(ActivityQa1QueueEntry entry) : DomainEvent
    {
        public ActivityQa1QueueEntry Entry { get; } = entry;
    }
}