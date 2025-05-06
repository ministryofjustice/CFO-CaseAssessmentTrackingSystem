using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class CreateTransitionEvent(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        ParticipantEnrolmentHistory history = ParticipantEnrolmentHistory.Create(notification.Item.Id, notification.To, notification.Reason, notification.AdditionalInformation);
        await unitOfWork.DbContext.ParticipantEnrolmentHistories.AddAsync(history, cancellationToken);
    }
}