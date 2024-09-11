using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RiskInformationCompleted(IUnitOfWork unitOfWork) : INotificationHandler<RiskInformationCompletedDomainEvent>
{
    public async Task Handle(RiskInformationCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var participant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == notification.Item.ParticipantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is not null)
        {
            participant.SetRiskDue(DateTime.UtcNow.AddDays(70));
            unitOfWork.DbContext.Participants.Update(participant);
        }

    }
}
