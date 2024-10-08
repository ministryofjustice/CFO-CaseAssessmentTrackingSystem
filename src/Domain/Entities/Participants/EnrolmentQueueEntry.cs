using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

/* ef core does not support the same type being in two tables. need explicit types */

public abstract class EnrolmentQueueEntry : OwnerPropertyEntity<Guid>, IMustHaveTenant
{
    private readonly List<Note> _notes = [];
    
    public bool IsAccepted { get; protected set; }
    public bool IsCompleted { get; protected set; }
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

    public abstract EnrolmentQueueEntry Accept();


    public abstract EnrolmentQueueEntry Return();
    

    public EnrolmentQueueEntry AddNote(string? message)
    {
        if (string.IsNullOrWhiteSpace(message) == false)
        {
            _notes.Add(new Note()
            {
                TenantId = TenantId,
                Message = message
            });
        }
        return this;
    }

    public string TenantId { get; set; } = default!;
}
