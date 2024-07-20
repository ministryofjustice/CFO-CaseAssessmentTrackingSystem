using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQa2QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa2QueueEntry() : this(string.Empty)
    {
        
    }
    
    private EnrolmentQa2QueueEntry(string participantId) : base(participantId)
    {
        this.AddDomainEvent(new EnrolmentQa2QueueCreatedDomainEvent(this));
    }
    
    public static EnrolmentQa2QueueEntry Create(string participantId)
    {
        return new EnrolmentQa2QueueEntry(participantId);
    }
    
}