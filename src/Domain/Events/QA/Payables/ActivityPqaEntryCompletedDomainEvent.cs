using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityPqaEntryCompletedDomainEvent(ActivityPqaQueueEntry entry) : DomainEvent
    {
        public ActivityPqaQueueEntry Entry { get; } = entry;
    }
}
