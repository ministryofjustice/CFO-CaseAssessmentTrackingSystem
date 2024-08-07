﻿using Cfo.Cats.Domain.Common.Entities;
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
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Participant()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static Participant CreateFrom(string id, string firstName, string lastName, DateTime dateOfBirth, string referralSource, string? referralComments)
    {
        Participant p = new Participant
        {
            ConsentStatus = ConsentStatus.PendingStatus,
            EnrolmentStatus = EnrolmentStatus.PendingStatus,
            Id = id,
            DateOfBirth = DateOnly.FromDateTime(dateOfBirth),
            FirstName = firstName,
            LastName = lastName,
            ReferralSource = referralSource,
            ReferralComments = referralComments,
            _currentLocationId = 1
        };
        
        p.AddDomainEvent(new ParticipantCreatedDomainEvent(p));
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

    public Location CurrentLocation { get; private set; }
    
    /// <summary>
    /// The location where this participant was enrolled.
    /// </summary>
    public Location? EnrolmentLocation { get; private set; }

    /// <summary>
    /// If the location differed from the current location
    /// </summary>
    public string? EnrolmentLocationJustification { get; private set; }

    public IReadOnlyCollection<Consent> Consents => _consents.AsReadOnly();

    public IReadOnlyCollection<RightToWork> RightToWorks => _rightToWorks.AsReadOnly();

    public IReadOnlyCollection<Note> Notes => _notes.AsReadOnly();

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

}
