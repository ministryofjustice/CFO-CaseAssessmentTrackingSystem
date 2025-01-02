using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities
{
    public class ActivityQa1QueueEntry : ActivityQueueEntry
    {
        private ActivityQa1QueueEntry() : this(Guid.Empty)
        {
        }

        private ActivityQa1QueueEntry(Guid activityId) : base(activityId)
            => AddDomainEvent(new ActivityQa1QueueCreatedDomainEvent(this));

        public static ActivityQa1QueueEntry Create(Guid activityId) => new(activityId);

        public override ActivityQueueEntry Accept()
        {
            IsAccepted = true;
            IsCompleted = true;
            AddDomainEvent(new ActivityQa1EntryCompletedDomainEvent(this));
            return this;
        }

        public override ActivityQueueEntry Return()
        {
            IsAccepted = false;
            IsCompleted = true;
            AddDomainEvent(new ActivityQa1EntryCompletedDomainEvent(this));
            return this;
        }
    }
}