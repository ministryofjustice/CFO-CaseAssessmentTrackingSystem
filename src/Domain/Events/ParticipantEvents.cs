using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class ParticipantCreatedDomainEvent(Participant participant) : DomainEvent
{
    public Participant Item = participant;
}

public sealed class ParticipantAssignedDomainEvent(Participant participant, string? from, string? to) : DomainEvent
{
    public Participant Item = participant;
    public string? FromOwner = from;
    public string? NewOwner = to;
}

public sealed class ParticipantTransitionedEvent(Participant participant, EnrolmentStatus from, EnrolmentStatus to) 
    : DomainEvent
{
    public Participant Item = participant;
    public EnrolmentStatus From = from;
    public EnrolmentStatus To = to;
}