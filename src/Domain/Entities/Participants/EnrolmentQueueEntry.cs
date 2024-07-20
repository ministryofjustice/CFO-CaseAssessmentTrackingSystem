using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

/* ef core does not support the same type being in two tables. need explicit types */
public class EnrolmentPqaQueueEntry : EnrolmentQueueEntry
{
    private EnrolmentPqaQueueEntry() : this(string.Empty)
    {
        
    }
    private EnrolmentPqaQueueEntry(string participantId)
        : base(participantId)
    {
        
    }
    

    public static EnrolmentPqaQueueEntry Create(string participantId)
    {
        return new EnrolmentPqaQueueEntry(participantId);
    }
}
public class EnrolmentQa1QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa1QueueEntry() : this(string.Empty)
    {
        
    }
    private EnrolmentQa1QueueEntry(string participantId) : base(participantId)
    {
    }
    
    public static EnrolmentQa1QueueEntry Create(string participantId)
    {
        return new EnrolmentQa1QueueEntry(participantId);
    }
    
}
public class EnrolmentQa2QueueEntry : EnrolmentQueueEntry
{
    private EnrolmentQa2QueueEntry() : this(string.Empty)
    {
        
    }
    
    private EnrolmentQa2QueueEntry(string participantId) : base(participantId)
    {
    }
    
    public static EnrolmentQa2QueueEntry Create(string participantId)
    {
        return new EnrolmentQa2QueueEntry(participantId);
    }
    
}
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

public abstract class EnrolmentQueueEntry : OwnerPropertyEntity<Guid>, IMustHaveTenant
{
    private readonly List<Note> _notes = [];
    
    public bool IsCompleted { get; private set; }
    public string ParticipantId { get; private set; }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private EnrolmentQueueEntry()
    {
        
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.    

    protected EnrolmentQueueEntry(string participantId)
    {
        Id = Guid.NewGuid();
        ParticipantId = participantId;
    }

    public virtual Participant? Participant { get; private set; }
    public virtual Tenant? Tenant { get; private set; }
    
    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();
    
    public EnrolmentQueueEntry AddNote(Note note)
    {
        if(_notes.Contains(note) is false)
        {
            _notes.Add(note);
        }

        return this;
    }

    public EnrolmentQueueEntry Complete()
    {
        IsCompleted = true;
        return this;
    }

    public string TenantId { get; set; } = default!;
}
