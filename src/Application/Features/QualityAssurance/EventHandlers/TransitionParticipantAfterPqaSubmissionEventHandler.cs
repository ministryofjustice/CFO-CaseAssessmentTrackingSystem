using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class TransitionParticipantAfterPqaSubmissionEventHandler 
    : INotificationHandler<EnrolmentPqaEntryCompletedDomainEvent>
{
    public Task Handle(EnrolmentPqaEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry.IsAccepted)
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.SubmittedToAuthorityStatus);
        }
        else
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.EnrollingStatus);
        }

        return Task.CompletedTask;
    }
}