using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Participant : OwnerPropertyEntity<string>
{
    private int _currentLocationId;
    private int? _enrolmentLocationId;
    private List<Consent> _consents = new();
    private List<RightToWork> _rightToWorks = new();
    private List<Note> _notes = new();
    private List<ExternalIdentifier> _externalIdentifiers = new();


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Participant()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static Participant CreateFrom(string id, string firstName, string? middleName, string lastName, string? gender, DateTime dateOfBirth, string referralSource, string? referralComments, int locationId)
    {
        Participant p = new Participant
        {
            ConsentStatus = ConsentStatus.PendingStatus,
            EnrolmentStatus = EnrolmentStatus.PendingStatus,
            Id = id,
            DateOfBirth = DateOnly.FromDateTime(dateOfBirth),
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Gender = gender,
            ReferralSource = referralSource,
            ReferralComments = referralComments,
            _currentLocationId = locationId
        };

        p.AddDomainEvent(new ParticipantCreatedDomainEvent(p));
        return p;
    }

    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string? Gender { get; private set; }
    public DateOnly DateOfBirth { get; private set; }

    public string ReferralSource { get; private set; }

    public string? ReferralComments { get; private set; }
    
    public EnrolmentStatus? EnrolmentStatus { get; private set; }
    
    public ConsentStatus? ConsentStatus { get; private set; }

    public Location CurrentLocation { get; private set; }
    
    /// <summary>
    /// The location where this participant was enrolled.
    /// </summary>
    public Location? EnrolmentLocation { get; private set; }

    /// <summary>
    /// If the location differed from the current location
    /// </summary>
    public string? EnrolmentLocationJustification { get; private set; }

    public string? AssessmentJustification { get; private set; }

    public string? FullName => string.Join(' ', [FirstName, MiddleName, LastName]);

    public Supervisor? Supervisor { get; private set; }

    public IReadOnlyCollection<Consent> Consents => _consents.AsReadOnly();

    public IReadOnlyCollection<RightToWork> RightToWorks => _rightToWorks.AsReadOnly();

    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

    public IReadOnlyCollection<ExternalIdentifier> ExternalIdentifiers => _externalIdentifiers.AsReadOnly();

    /// <summary>
    /// Transitions this participant to the new enrolment status, if valid
    /// </summary>
    /// <param name="to">The new enrolment status</param>
    /// <returns>This entity.</returns>
    /// <exception cref="InvalidEnrolmentTransition">If the new enrolment status is not valid</exception>
    public Participant TransitionTo(EnrolmentStatus to)
    {
        if (EnrolmentStatus!.CanTransitionTo(to))
        {
            AddDomainEvent(new ParticipantTransitionedDomainEvent(this, EnrolmentStatus, to));
            EnrolmentStatus = to;
            return this;
        }
        throw new InvalidEnrolmentTransition(EnrolmentStatus, to);
    }

    public Participant SetEnrolmentLocation(int locationId, string? justificationReason)
    {
        if (_enrolmentLocationId != locationId)
        {
            _enrolmentLocationId = locationId;
            EnrolmentLocationJustification = justificationReason;
        }
        return this;
    }

    public Participant SetAssessmentJustification(string? justification)
    {
        AssessmentJustification = justification;
        return this;
    }

    /// <summary>
    /// Assigns the participant to the new user id
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    public Participant AssignTo(string? to)
    {
        if (to != OwnerId)
        {
            AddDomainEvent(new ParticipantAssignedDomainEvent(this, OwnerId, to));
            OwnerId = to;
        }
        return this;
    }

    public Participant AddConsent(DateTime consentDate, Guid documentId)
    {
        _consents.Add(Consent.Create( Id, consentDate, documentId ));
        return this;
    }

    public Participant AddRightToWork(DateTime validFrom, DateTime validTo, Guid documentId)
    {
        _rightToWorks.Add(RightToWork.Create(Id, validFrom, validTo, documentId ));
        return this;
    }

    public Participant MoveToLocation(Location to)
    {
        if (CurrentLocation.Id != to.Id)
        {
            AddDomainEvent(new ParticipantMovedDomainEvent(this, CurrentLocation, to));
            CurrentLocation = to;
            _currentLocationId = to.Id;
        }
        return this;
    }

    public Participant AddNote(Note note)
    {
        if (_notes.Contains(note) is false)
        {
            _notes.Add(note);
        }

        return this;
    }

    public Participant AddOrUpdateDateOfBirth(DateOnly dateOfBirth)
    {
        if(DateOfBirth != dateOfBirth)
        {
            DateOfBirth = dateOfBirth;
            AddDomainEvent(new ParticipantDateOfBirthChangedDomainEvent(this, DateOfBirth, dateOfBirth));
        }

        return this;
    }

    public Participant AddOrUpdateExternalIdentifier(ExternalIdentifier newIdentifier)
    {
        if(_externalIdentifiers.Contains(newIdentifier))
        {
            return this;
        }

        var identifier = _externalIdentifiers.Find(x => x.Type == newIdentifier.Type);

        if(identifier is { Type.IsExclusive: true } )
        {
            _externalIdentifiers.Remove(identifier);
            AddDomainEvent(new ParticipantIdentifierChangedDomainEvent(this, identifier, newIdentifier));
        }

        _externalIdentifiers.Add(newIdentifier);

        return this;
    }

    public Participant AddOrUpdateGender(string? gender)
    {
        if (string.Equals(Gender, gender, StringComparison.OrdinalIgnoreCase) is false)
        {
            Gender = gender;
            AddDomainEvent(new ParticipantGenderChangedDomainEvent(this, Gender, gender));
        }

        return this;
    }

    public Participant AddOrUpdateNameInformation(string firstName, string? middleName, string lastName)
    {
        bool nameHasChanged = false;

        string? currentName = FullName;

        if(string.Equals(FirstName, firstName, StringComparison.OrdinalIgnoreCase) is false)
        {
            FirstName = firstName;
            nameHasChanged = true;
        }

        if (string.Equals(MiddleName, middleName, StringComparison.OrdinalIgnoreCase) is false)
        {
            MiddleName = middleName;
            nameHasChanged = true;
        }

        if (string.Equals(LastName, lastName, StringComparison.OrdinalIgnoreCase) is false)
        {
            LastName = lastName;
            nameHasChanged = true;
        }

        if (nameHasChanged)
        {
            AddDomainEvent(new ParticipantNameChangedDomainEvent(this, currentName, FullName));
        }

        return this;
    }

    public Participant AddOrUpdateSupervisor(Supervisor? newSupervisor)
    {
        Supervisor = newSupervisor;
        return this;
    }

    public Participant ApproveConsent()
    {
        if(ConsentStatus == ConsentStatus.PendingStatus)
        {
            ConsentStatus = ConsentStatus.GrantedStatus;
        }
        return this;
    }

}
