using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RiskInformationReviewed(IUnitOfWork unitOfWork) : INotificationHandler<RiskInformationReviewedDomainEvent>
{
    public async Task Handle(RiskInformationReviewedDomainEvent notification, CancellationToken cancellationToken)
    {
        var participant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == notification.Item.ParticipantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is not null)
        {         
            //If risk still not completed class it as initial review and keep review date as 14 days
            if (participant.EnrolmentStatus == EnrolmentStatus.EnrollingStatus && notification.Item.ReviewReason == RiskReviewReason.NoRiskInformationAvailable)
            {
                participant.SetRiskDue(DateTime.UtcNow.AddDays(14), RiskDueReason.InitialReview);
            }
            else
            {
                participant.SetRiskDue(DateTime.UtcNow.AddDays(70), RiskDueReason.TenWeekReview);
            }
        }
    }
}