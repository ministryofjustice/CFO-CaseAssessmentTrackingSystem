using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class TransitionParticipantAfterEscalationSubmissionEventHandler
    : INotificationHandler<EnrolmentEscalationEntryCompletedDomainEvent>
{
    public Task Handle(EnrolmentEscalationEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
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
                .TransitionTo(EnrolmentStatus.SubmittedToProviderStatus);
        }

        return Task.CompletedTask;
    }
}