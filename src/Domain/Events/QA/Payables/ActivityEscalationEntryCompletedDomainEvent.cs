using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityEscalationEntryCompletedDomainEvent(ActivityEscalationQueueEntry entry) : DomainEvent
    {
        public ActivityEscalationQueueEntry Entry { get; } = entry;
    }
}
