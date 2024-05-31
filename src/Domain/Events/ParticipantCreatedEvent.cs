using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public class ParticipantCreatedEvent(Participant participant) : DomainEvent
{
    public Participant Item = participant;
}

public class ParticipantAssignedEvent(Participant participant, int? from, int? to) : DomainEvent
{
    public Participant Item = participant;
    public int? FromOwner = from;
    public int? NewOwner = to;
}