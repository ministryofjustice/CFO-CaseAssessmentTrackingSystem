using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQa2QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa2QueueEntry() 
        : base()
    {
        
    }
    
    public bool IsEscalated { get; private set; }

    public EnrolmentQa2QueueEntry(string participantId, string tenantId, string supportWorkerId, DateTime consentDate) 
        : base(participantId, tenantId, supportWorkerId, consentDate)
    {
        AddDomainEvent(new EnrolmentQa2QueueCreatedDomainEvent(this));
    }

    public override EnrolmentQueueEntry Accept()
    {
        IsAccepted = true;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentQa2EntryCompletedDomainEvent(this));
        return this;
    }

    public override EnrolmentQueueEntry Return()
    {
        IsAccepted = false;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentQa2EntryCompletedDomainEvent(this));
        return this;
    }
    
    public EnrolmentQueueEntry Escalate()
    {
        IsCompleted = true;
        IsAccepted = false;
        IsEscalated = true;
        AddDomainEvent(new EnrolmentQa2EntryEscalatedDomainEvent(this));
        return this;
    }

}