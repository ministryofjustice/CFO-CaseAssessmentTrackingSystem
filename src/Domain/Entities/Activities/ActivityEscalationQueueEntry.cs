using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities
{    
    public class ActivityEscalationQueueEntry : ActivityQueueEntry
    {
        private ActivityEscalationQueueEntry() : base(Guid.Empty)
        {
        }

        private ActivityEscalationQueueEntry(Guid activityId, string tenantId, Participant participant) : base(activityId)
        {
            TenantId = tenantId;
            Participant = participant;
            //todo: Add event?
        }

        public static ActivityEscalationQueueEntry Create(Guid activityId, string tenantId, Participant participant)
            => new(activityId, tenantId, participant);

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