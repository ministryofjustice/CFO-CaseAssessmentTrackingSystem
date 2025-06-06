﻿using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

/* ef core does not support the same type being in two tables. need explicit types */

public abstract class EnrolmentQueueEntry : OwnerPropertyEntity<Guid>
{
    private readonly List<EnrolmentQueueEntryNote> _notes = [];
    
    public bool IsAccepted { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public string ParticipantId { get; private set; }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected EnrolmentQueueEntry()
    {
        
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.    

    protected EnrolmentQueueEntry(string participantId, string tenantId, string supportWorkerId, DateTime consentDate)
    {
        Id = Guid.CreateVersion7();
        ParticipantId = participantId;
        TenantId = tenantId;
        SupportWorkerId = supportWorkerId;
        ConsentDate = consentDate;
    }

    public virtual Participant? Participant { get; private set; }
    public virtual Tenant? Tenant { get; private set; }
    
    public IReadOnlyCollection<EnrolmentQueueEntryNote> Notes => _notes.AsReadOnly();

    public abstract EnrolmentQueueEntry Accept();

    public abstract EnrolmentQueueEntry Return();
    
    public EnrolmentQueueEntry AddNote(string? message, bool isExternal = false)
    {
        if (string.IsNullOrWhiteSpace(message) == false)
        {
            _notes.Add(new EnrolmentQueueEntryNote()
            {
                TenantId = TenantId,
                Message = message,
                IsExternal = isExternal
            });
        }
        return this;
    }

    public string TenantId { get;  private set; }

    public string SupportWorkerId { get; private set; }

    public virtual ApplicationUser? SupportWorker { get; private set; }

    public DateTime ConsentDate { get; private set; }
}
