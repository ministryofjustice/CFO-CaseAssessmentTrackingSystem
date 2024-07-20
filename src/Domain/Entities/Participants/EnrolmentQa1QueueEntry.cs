using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQa1QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa1QueueEntry() : this(string.Empty)
    {
        
    }
    private EnrolmentQa1QueueEntry(string participantId) : base(participantId)
    {
        this.AddDomainEvent(new EnrolmentQa1QueueCreatedDomainEvent(this));
    }
    
    public static EnrolmentQa1QueueEntry Create(string participantId)
    {
        return new EnrolmentQa1QueueEntry(participantId);
    }
    
}