using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RecordActiveStatusChange(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantActiveStatusChangedDomainEvent>
{
    public async Task Handle(ParticipantActiveStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var history = ParticipantActiveStatusHistory.Create(notification.Item.Id, notification.From, notification.To, notification.Occurred);
        await unitOfWork.DbContext.ParticipantActiveStatusHistories.AddAsync(history, cancellationToken);
    }
}
