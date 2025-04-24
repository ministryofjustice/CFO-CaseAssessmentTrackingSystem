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
            participant.SetRiskDue(DateTime.UtcNow.AddDays(70), RiskDueReason.TenWeekReview);
        }
    }
}