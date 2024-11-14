using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class QueueIncomingTransferOnMovement(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var locations = await unitOfWork.DbContext.Locations
            .Where(l => new[] { notification.From.Id, notification.To.Id }.Contains(l.Id))
            .Include(l => l.Contract)
            .ToDictionaryAsync(l => l.Id, cancellationToken);

        var (from, to) = (
            locations[notification.From.Id],
            locations[notification.To.Id]
        );

        var incomingActiveTransfer = await unitOfWork.DbContext.ParticipantIncomingTransferQueue
            .Where(q => q.ParticipantId == notification.Item.Id && q.Completed == false)
            .SingleOrDefaultAsync(cancellationToken);

        // Complete active transfer
        incomingActiveTransfer?.Complete();

        // Only publish incoming transfer if (destination/target) contract exists
        if (to.Contract is not null)
        {
            var incomingTransfer = ParticipantIncomingTransferQueueEntry.Create(
                notification.Item.Id,
                from,
                to,
                from.Contract,
                to.Contract,
                DateTime.UtcNow);

            await unitOfWork.DbContext.ParticipantIncomingTransferQueue.AddAsync(incomingTransfer, cancellationToken);
        }
    }
}
