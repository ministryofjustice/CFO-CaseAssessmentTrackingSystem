using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events.QA.Enrolments;

public sealed class EnrolmentPqaEntryCompletedDomainEvent(EnrolmentPqaQueueEntry entry) : DomainEvent
{
    public EnrolmentPqaQueueEntry Entry { get; } = entry;
}