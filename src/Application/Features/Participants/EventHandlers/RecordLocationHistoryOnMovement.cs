using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RecordLocationHistoryOnMovement(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var dateTime = DateTime.UtcNow;

        var lastMovement = await unitOfWork.DbContext.ParticipantLocationHistories
            .Where(h => h.ParticipantId == notification.Item.Id)
            .OrderByDescending(h => h.From)
            .FirstAsync(cancellationToken);

        lastMovement.WithTo(dateTime);

        var history = ParticipantLocationHistory.Create(
            notification.Item.Id, 
            notification.To.Id,
            dateTime);

        await unitOfWork.DbContext.ParticipantLocationHistories.AddAsync(history, cancellationToken);
    }
}
