﻿using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentPqaQueueEntry : EnrolmentQueueEntry
{
    private EnrolmentPqaQueueEntry() 
        : this(string.Empty)
    {
    }
    
    private EnrolmentPqaQueueEntry(string participantId)
        : base(participantId) =>
        AddDomainEvent(new EnrolmentPqaQueueCreatedDomainEvent(this));

    public static EnrolmentPqaQueueEntry Create(string participantId) => new(participantId);

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