using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Activity
{
    public sealed class ActivityPqaEntryCompletedDomainEvent(ActivityPqaQueueEntry entry) : DomainEvent
    {
        public ActivityPqaQueueEntry Entry { get; } = entry;
    }
}
