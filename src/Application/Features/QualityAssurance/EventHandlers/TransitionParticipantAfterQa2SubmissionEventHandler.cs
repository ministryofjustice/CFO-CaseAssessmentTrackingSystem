﻿using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class TransitionParticipantAfterQa2SubmissionEventHandler : INotificationHandler<EnrolmentQa2EntryCompletedDomainEvent>
{
    public Task Handle(EnrolmentQa2EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry.IsAccepted)
        {
            notification.Entry
                .Participant!
                .ApproveEnrolment();
        }
        else
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.SubmittedToProviderStatus, null, null);
        }

        return Task.CompletedTask;
    }
}