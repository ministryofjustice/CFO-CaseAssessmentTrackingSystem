using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentPqaQueueEntry : EnrolmentQueueEntry
{
    private EnrolmentPqaQueueEntry() : base()
    {
    }

    public EnrolmentPqaQueueEntry(string participantId, string tenantId, string supportWorkerId, DateTime consentDate)
        : base(participantId, tenantId, supportWorkerId, consentDate)
    {
        AddDomainEvent(new EnrolmentPqaQueueCreatedDomainEvent(this));
    }

    public override EnrolmentQueueEntry Accept()
    {
        IsAccepted = true;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentPqaEntryCompletedDomainEvent(this));
        return this;
    }

    public override EnrolmentQueueEntry Return()
    {
        IsAccepted = false;
        IsCompleted = true;
        AddDomainEvent(new EnrolmentPqaEntryCompletedDomainEvent(this));
        return this;
    }

}