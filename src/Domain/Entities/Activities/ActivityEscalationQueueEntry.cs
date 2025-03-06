using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityEscalationQueueEntry : ActivityQueueEntry
{
    private ActivityEscalationQueueEntry() : base()
    {
    }
         
    public ActivityEscalationQueueEntry(Guid activityId, string tenantId, string supportWorkerId, DateTime originalPQASubmissionDate, string participantId)
      : base(activityId, tenantId, supportWorkerId, originalPQASubmissionDate, participantId)
    {            
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