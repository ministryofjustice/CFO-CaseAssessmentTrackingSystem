using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantMovedLocation(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var history = ParticipantLocationHistory.Create(
            notification.Item.Id, 
            notification.To.Id, 
            DateTime.UtcNow);

        await unitOfWork.DbContext.ParticipantLocationHistories.AddAsync(history, cancellationToken);
    }
}
