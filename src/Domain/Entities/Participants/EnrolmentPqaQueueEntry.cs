using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentPqaQueueEntry : EnrolmentQueueEntry
{
    private EnrolmentPqaQueueEntry() : this(string.Empty)
    {
    }
    private EnrolmentPqaQueueEntry(string participantId)
        : base(participantId)
    {
        this.AddDomainEvent(new EnrolmentPqaQueueCreatedDomainEvent(this));   
    }
    

    public static EnrolmentPqaQueueEntry Create(string participantId)
    {
        return new EnrolmentPqaQueueEntry(participantId);
    }
}