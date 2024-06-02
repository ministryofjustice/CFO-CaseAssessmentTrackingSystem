using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class ParticipantCreatedEvent(Participant participant) : DomainEvent
{
    public Participant Item = participant;
}

public sealed class ParticipantAssignedEvent(Participant participant, int? from, int? to) : DomainEvent
{
    public Participant Item = participant;
    public int? FromOwner = from;
    public int? NewOwner = to;
}