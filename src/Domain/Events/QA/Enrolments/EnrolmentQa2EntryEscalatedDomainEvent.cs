using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events.QA.Enrolments;

public sealed class EnrolmentQa2EntryEscalatedDomainEvent(EnrolmentQa2QueueEntry entry) : DomainEvent
{
    public EnrolmentQa2QueueEntry Entry { get; } = entry;
}

