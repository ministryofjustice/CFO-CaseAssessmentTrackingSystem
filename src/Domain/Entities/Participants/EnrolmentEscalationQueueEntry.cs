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
}