using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities
{    
    public class ActivityEscalationQueueEntry : ActivityQueueEntry
    {
        private ActivityEscalationQueueEntry() : this(Guid.Empty)
        {
        }

        private ActivityEscalationQueueEntry(Guid activityId) : base(activityId)
        {
        }

        public static ActivityEscalationQueueEntry Create(Guid activityId)
        {
            return new ActivityEscalationQueueEntry(activityId);
        }

        public override ActivityQueueEntry Accept()
        {
            IsAccepted = true;
            IsCompleted = true;
            AddDomainEvent(new ActivityEscalationEntryCompletedDomainEvent(this));
            return this;
        }

        public override ActivityQueueEntry Return()
        {
            IsAccepted = false;
            IsCompleted = true;
            AddDomainEvent(new ActivityEscalationEntryCompletedDomainEvent(this));
            return this;
        }
    }
}