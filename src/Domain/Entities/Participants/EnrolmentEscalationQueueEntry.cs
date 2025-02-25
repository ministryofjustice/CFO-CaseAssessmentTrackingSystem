using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentEscalationQueueEntry : EnrolmentQueueEntry
{
    private EnrolmentEscalationQueueEntry() 
        : base()
    {
        
    }
    public EnrolmentEscalationQueueEntry(string participantId, string tenantId, string supportWorkerId, DateTime consentDate) 
        : base(participantId, tenantId, supportWorkerId, consentDate)
    {
    }
    

    public override EnrolmentQueueEntry Accept()
    {
        IsAccepted = true;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentEscalationEntryCompletedDomainEvent(this));
        return this;
    }

    public override EnrolmentQueueEntry Return()
    {
        IsAccepted = false;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentEscalationEntryCompletedDomainEvent(this));
        return this;
    }
}