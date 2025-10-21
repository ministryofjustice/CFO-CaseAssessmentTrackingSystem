using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class ParticipantCreatedDomainEvent(Participant participant, int locationId) : DomainEvent
{
    public Participant Item { get; } = participant;
    public int LocationId { get; } = locationId;
}

public sealed class ParticipantAssignedDomainEvent(Participant participant, string? from, string? to, int currentLocationId) : DomainEvent
{
    public Participant Item { get; } = participant;
    public string? FromOwner { get; } = from;
    public string? NewOwner { get; } = to;
    public int CurrentLocationId { get; } = currentLocationId;
}

public sealed class ParticipantTransitionedDomainEvent(Participant participant, EnrolmentStatus from, EnrolmentStatus to, string? reason, string? additionalInformation)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public EnrolmentStatus From { get; } = from;
    public EnrolmentStatus To { get; } = to;
    public string? Reason { get; } = reason;
    public string? AdditionalInformation { get; } = additionalInformation;
}

public sealed class ParticipantMovedDomainEvent(Participant participant, Location from, Location to, string? ownerId)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public Location From { get; } = from;
    public Location To { get; } = to;
    public string? ParticipantOwnerIdPreMovement { get; } = ownerId;
}

public sealed class ParticipantDateOfBirthChangedDomainEvent(Participant participant, DateOnly from, DateOnly to)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public DateOnly From { get; } = from;
    public DateOnly To { get; } = to;
}

public sealed class ParticipantGenderChangedDomainEvent(Participant participant, string? from, string? to)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public string? From { get; } = from;
    public string? To { get; } = to;
}

public sealed class ParticipantNationalityChangedDomainEvent(Participant participant, string? from, string? to)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public string? From { get; } = from;
    public string? To { get; } = to;
}

public sealed class ParticipantIdentifierChangedDomainEvent(Participant participant, ExternalIdentifier from, ExternalIdentifier to)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public ExternalIdentifier From { get; } = from;
    public ExternalIdentifier To { get; } = to;
}

public sealed class ParticipantNameChangedDomainEvent(Participant participant, string? from, string? to)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public string? From { get; } = from;
    public string? To { get; } = to;
}

public sealed class ParticipantActiveStatusChangedDomainEvent(Participant item, bool from, bool to, DateOnly occured) 
    : DomainEvent
{
    public Participant Item { get; } = item;
    public bool From { get; } = from;
    public bool To { get; } = to;
    public DateOnly Occurred { get; } = occured;
}

public sealed class ParticipantRegistrationDetailsChangedDomainEvent(Participant item)
    : DomainEvent
{
    public Participant Item { get; } = item;
}

public sealed class ParticipantEnrolmentApprovedDomainEvent(Participant item) : DomainEvent
{
    public Participant Item { get; } = item;
}