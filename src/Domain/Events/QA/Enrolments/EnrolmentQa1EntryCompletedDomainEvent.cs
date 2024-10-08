using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events.QA.Enrolments;

public sealed class EnrolmentQa1EntryCompletedDomainEvent(EnrolmentQa1QueueEntry entry) : DomainEvent
{
    public EnrolmentQa1QueueEntry Entry { get; } = entry;
}