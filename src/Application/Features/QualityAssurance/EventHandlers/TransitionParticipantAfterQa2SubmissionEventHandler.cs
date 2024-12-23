using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class TransitionParticipantAfterQa2SubmissionEventHandler(IUnitOfWork unitOfWork, IRightToWorkSettings rtwSettings)
    : INotificationHandler<EnrolmentQa2EntryCompletedDomainEvent>
{
    public async Task Handle(EnrolmentQa2EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Transition back to PQA if right to work has been provided (and is required).

        bool HasActiveRightToWork = false;
        bool IsBritish = rtwSettings.NationalitiesExempted.Any(s => s.Equals(notification.Entry.Participant!.Nationality!, StringComparison.OrdinalIgnoreCase));
        if (IsBritish is false)
        {
            HasActiveRightToWork = await unitOfWork.DbContext.Participants
                .Where(x => x.Id == notification.Entry.Participant!.Id)
                .SelectMany(p => p.RightToWorks)
                .AnyAsync(x => x.Lifetime.EndDate >= DateTime.Now.Date, cancellationToken);
        }

        bool IsRightToWorkRequired = IsBritish is false && HasActiveRightToWork is false;

        // Approve consent if accepted
        if (notification.Entry.IsAccepted)
        {
            notification.Entry.Participant!.ApproveConsent();
        }

        // Approve enrolment if QA2 accepts AND right to work is not required
        if (notification.Entry.IsAccepted && IsRightToWorkRequired is false)
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.ApprovedStatus);
        }
        else
        {
            notification.Entry
                .Participant!
                .TransitionTo(EnrolmentStatus.SubmittedToProviderStatus);
        }

        //return Task.CompletedTask;
    }
}