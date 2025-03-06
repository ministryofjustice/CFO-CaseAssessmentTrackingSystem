using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityPqaQueueEntry : ActivityQueueEntry
{
    private ActivityPqaQueueEntry() : base()
    {
    }

    public ActivityPqaQueueEntry(Guid activityId, string tenantId, string supportWorkerId, DateTime originalPQASubmissionDate,string participantId)
        : base(activityId, tenantId, supportWorkerId, originalPQASubmissionDate, participantId)
    {
        AddDomainEvent(new ActivityPqaQueueCreatedDomainEvent(this));
    }
    
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