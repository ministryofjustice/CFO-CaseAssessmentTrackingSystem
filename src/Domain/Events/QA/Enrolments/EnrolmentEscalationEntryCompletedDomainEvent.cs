using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events.QA.Enrolments;

public sealed class EnrolmentEscalationEntryCompletedDomainEvent(EnrolmentEscalationQueueEntry entry) : DomainEvent
{
    public EnrolmentEscalationQueueEntry Entry { get; } = entry;
}