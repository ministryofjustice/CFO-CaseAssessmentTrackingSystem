using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityQa1QueueEntry : ActivityQueueEntry
{
    private ActivityQa1QueueEntry() : base()
    {
    }

    public ActivityQa1QueueEntry(Guid activityId, string tenantId, string supportWorkerId, DateTime originalPQASubmissionDate,string participantId)
   : base(activityId, tenantId, supportWorkerId, originalPQASubmissionDate, participantId)
   => AddDomainEvent(new ActivityQa1QueueCreatedDomainEvent(this));

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