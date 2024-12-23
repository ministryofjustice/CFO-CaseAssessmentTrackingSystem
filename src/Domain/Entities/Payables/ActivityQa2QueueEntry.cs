using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Payables
{    
    public class ActivityQa2QueueEntry : ActivityQueueEntry
    {
        private ActivityQa2QueueEntry() : base(Guid.Empty)
        {
        }

        public bool IsEscalated { get; private set; }

        private ActivityQa2QueueEntry(Guid activityId, string tenantId, Participant participant) : base(activityId)
        {
            TenantId = tenantId;
            Participant = participant;
            AddDomainEvent(new ActivityQa2QueueCreatedDomainEvent(this));
        }

        public static ActivityQa2QueueEntry Create(Guid activityId, string tenantId, Participant participant)
            => new(activityId, tenantId, participant);

        public override ActivityQueueEntry Accept()
        {
            IsAccepted = true;
            IsCompleted = true;
            AddDomainEvent(new ActivityQa2EntryCompletedDomainEvent(this));
            return this;
        }

        public override ActivityQueueEntry Return()
        {
            IsAccepted = false;
            IsCompleted = true;
            AddDomainEvent(new ActivityQa2EntryCompletedDomainEvent(this));
            return this;
        }

        public ActivityQueueEntry Escalate()
        {
            IsCompleted = true;
            IsAccepted = false;
            IsEscalated = true;
            AddDomainEvent(new ActivityQa2EntryEscalatedDomainEvent(this));
            return this;
        }
    }
}