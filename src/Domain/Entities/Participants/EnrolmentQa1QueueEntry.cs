using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQa1QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa1QueueEntry() 
        : base()
    {
        
    }

    public EnrolmentQa1QueueEntry(string participantId, string tenantId, string supportWorkerId, DateTime consentDate) 
        : base(participantId, tenantId, supportWorkerId, consentDate) 
        => AddDomainEvent(new EnrolmentQa1QueueCreatedDomainEvent(this));

    public override EnrolmentQueueEntry Accept()
    {
        IsAccepted = true;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentQa1EntryCompletedDomainEvent(this));
        return this;
    }

    public override EnrolmentQueueEntry Return()
    {
        IsAccepted = false;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentQa1EntryCompletedDomainEvent(this));
        return this;
    }

}