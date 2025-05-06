using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class VerifyRightToWorkOnApproval(IUnitOfWork unitOfWork, IRightToWorkSettings rtwSettings) : INotificationHandler<ParticipantEnrolmentApprovedDomainEvent>
{
    public async Task Handle(ParticipantEnrolmentApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
        bool hasActiveRightToWork = false;

        bool isExempt = rtwSettings.NationalitiesExempted.Any(s => s.Equals(notification.Item!.Nationality!, StringComparison.OrdinalIgnoreCase));

        if (isExempt is false)
        {
            hasActiveRightToWork = await unitOfWork.DbContext.Participants
                .Where(x => x.Id == notification.Item!.Id)
                .SelectMany(p => p.RightToWorks)
                .AnyAsync(x => x.Lifetime.EndDate >= DateTime.Now.Date, cancellationToken);
        }

        bool isRightToWorkRequired = isExempt is false && hasActiveRightToWork is false;

        if (isRightToWorkRequired)
        {
            notification.Item.TransitionTo(EnrolmentStatus.SubmittedToProviderStatus, null, null);
        }
        else
        {
            notification.Item.TransitionTo(EnrolmentStatus.ApprovedStatus, null, null);
        }
    }
}