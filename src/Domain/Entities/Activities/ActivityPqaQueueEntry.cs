using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities
{
    public class ActivityPqaQueueEntry : ActivityQueueEntry
    {
        private ActivityPqaQueueEntry()
            : this(Guid.Empty)
        {
        }

        private ActivityPqaQueueEntry(Guid activityId)
            : base(activityId) =>
            AddDomainEvent(new ActivityPqaQueueCreatedDomainEvent(this));

        public static ActivityPqaQueueEntry Create(Guid activityId) => new(activityId);

        public override ActivityQueueEntry Accept()
        {
            IsAccepted = true;
            IsCompleted = true;
            AddDomainEvent(new ActivityPqaEntryCompletedDomainEvent(this));
            return this;
        }

        public override ActivityQueueEntry Return()
        {
            IsAccepted = false;
            IsCompleted = true;
            AddDomainEvent(new ActivityPqaEntryCompletedDomainEvent(this));
            return this;
        }
    }
}
