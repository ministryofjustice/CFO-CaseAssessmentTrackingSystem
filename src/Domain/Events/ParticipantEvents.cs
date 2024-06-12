using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class ParticipantCreatedDomainEvent(Participant participant) : DomainEvent
{
    public Participant Item { get; } = participant;
}

public sealed class ParticipantAssignedDomainEvent(Participant participant, string? from, string? to) : DomainEvent
{
    public Participant Item { get; } = participant;
    public string? FromOwner { get; } = from;
    public string? NewOwner { get; } = to;
}

public sealed class ParticipantTransitionedEvent(Participant participant, EnrolmentStatus from, EnrolmentStatus to) 
    : DomainEvent
{
    public Participant Item { get; }= participant;
    public EnrolmentStatus From { get; } = from;
    public EnrolmentStatus To { get; }= to;
}

public sealed class ParticipantMovedDomainEvent(Participant participant, Location from, Location to)
    : DomainEvent
{
    public Participant Item { get; } = participant;
    public Location From { get; } = from;
    public Location To { get; } = to;
}