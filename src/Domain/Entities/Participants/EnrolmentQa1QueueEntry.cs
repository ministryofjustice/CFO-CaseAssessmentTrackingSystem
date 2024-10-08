using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQa1QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa1QueueEntry() : this(string.Empty)
    {
        
    }
    private EnrolmentQa1QueueEntry(string participantId) : base(participantId) 
        => AddDomainEvent(new EnrolmentQa1QueueCreatedDomainEvent(this));

    public static EnrolmentQa1QueueEntry Create(string participantId) => new(participantId);

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