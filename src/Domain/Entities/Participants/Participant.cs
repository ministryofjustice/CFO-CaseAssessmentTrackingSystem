using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Participant : OwnerPropertyEntity<string>
{

    // EF Core Constructor
    private Participant()
    {
    }

    public static Participant CreateFrom(Candidate candidate, string referralSource, string? referralComments)
    {
        Participant p = new Participant
        {
            ConsentStatus = ConsentStatus.PendingStatus,
            EnrolmentStatus = EnrolmentStatus.PendingStatus,
            Id = candidate.Id,
            DateOfBirth = DateOnly.FromDateTime(candidate.DateOfBirth),
            FirstName = candidate.FirstName,
            LastName = candidate.LastName,
            ReferralSource = referralSource,
            ReferralComments = referralComments
        };

        p.AddDomainEvent(new ParticipantCreatedEvent(p));

        return p;
    }

    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    
    public string ReferralSource { get; private set; }

    public string? ReferralComments { get; private set; }
    
    public EnrolmentStatus? EnrolmentStatus { get; private set; }
    
    public ConsentStatus? ConsentStatus { get; private set; }

    public Candidate Candidate { get; set; } = default!;

    public void AssignTo(int? userId)
    {
        if (userId != OwnerId)
        {
            AddDomainEvent(new ParticipantAssignedEvent(this, OwnerId, userId));
            OwnerId = userId;
        }
    }
}
