using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Payables
{    
    public class ActivityQa2QueueEntry : ActivityQueueEntry
    {
        private ActivityQa2QueueEntry() : this(Guid.Empty)
        {
        }

        public bool IsEscalated { get; private set; }

        private ActivityQa2QueueEntry(Guid activityId) : base(activityId)
            => AddDomainEvent(new ActivityQa2QueueCreatedDomainEvent(this));

        public static ActivityQa2QueueEntry Create(Guid activityId)
            => new(activityId);

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