using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class EnrolmentQueueEntryCompletedDomainEvent(EnrolmentQueueEntry entry)
    : DomainEvent
{
    public EnrolmentQueueEntry Entry { get; } = entry;
}
