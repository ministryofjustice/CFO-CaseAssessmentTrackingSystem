using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityQa2QueueEntry : ActivityQueueEntry
{
    private ActivityQa2QueueEntry() : base()
    {
    }

    public bool IsEscalated { get; private set; }

    public ActivityQa2QueueEntry(Guid activityId, string tenantId, string supportWorkerId, DateTime consentDate,string participantId) 
        : base(activityId, tenantId, supportWorkerId, consentDate, participantId)
    {
        AddDomainEvent(new ActivityQa2QueueCreatedDomainEvent(this));
    }
    
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