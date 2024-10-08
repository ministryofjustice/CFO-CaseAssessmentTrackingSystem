using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentQa2QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa2QueueEntry() : this(string.Empty)
    {
        
    }
    
    public bool IsEscalated { get; private set; }

    private EnrolmentQa2QueueEntry(string participantId) : base(participantId) 
        => AddDomainEvent(new EnrolmentQa2QueueCreatedDomainEvent(this));

    public static EnrolmentQa2QueueEntry Create(string participantId) 
        => new(participantId);

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