using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class TransitionParticipantAfterQa2SubmissionEventHandler 
    : INotificationHandler<EnrolmentQa2EntryCompletedDomainEvent>
{
    public Task Handle(EnrolmentQa2EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry.IsAccepted)
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.ApprovedStatus)
                .ApproveConsent();
        }
        else
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.SubmittedToProviderStatus);
        }

        return Task.CompletedTask;
    }
}