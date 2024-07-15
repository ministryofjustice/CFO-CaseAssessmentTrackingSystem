using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantTransitionedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        ParticipantEnrolmentHistory history = ParticipantEnrolmentHistory.Create(notification.Item.Id, notification.To);
        await unitOfWork.DbContext.ParticipantEnrolmentHistories.AddAsync(history, cancellationToken);
    }
}