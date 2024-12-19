using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Payables
{
    public class ActivityPqaQueueEntry : ActivityQueueEntry
    {
        private ActivityPqaQueueEntry()
            : this(Guid.Empty,string.Empty)
        {
        }

        private ActivityPqaQueueEntry(Guid activityId,string participantId)
            : base(activityId, participantId) =>
            AddDomainEvent(new ActivityPqaQueueCreatedDomainEvent(this));

        public static ActivityPqaQueueEntry Create(Guid activityId,string participantId) => new(activityId, participantId);

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
