using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentEscalationQueueEntry : EnrolmentQueueEntry
{
    private EnrolmentEscalationQueueEntry() : this(string.Empty)
    {
        
    }
    private EnrolmentEscalationQueueEntry(string participantId) : base(participantId)
    {
    }
    
    public static EnrolmentEscalationQueueEntry Create(string participantId)
    {
        return new EnrolmentEscalationQueueEntry(participantId);
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