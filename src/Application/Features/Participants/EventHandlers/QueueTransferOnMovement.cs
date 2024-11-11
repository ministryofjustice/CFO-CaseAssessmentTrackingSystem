using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class QueueTransferOnMovement(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var locations = await unitOfWork.DbContext.Locations
            .Where(l => new[] { notification.From.Id, notification.To.Id }.Contains(l.Id))
            .Include(l => l.Contract)
            .ToListAsync(cancellationToken);

        var from = locations.First(l => l.Id == notification.From.Id);
        var to = locations.First(l => l.Id == notification.To.Id);

        // Only publish outgoing transfer if (source/original) contract exists
        if (from is { Contract: not null })
        {
            var outgoingTransfer = ParticipantOutgoingTransferQueueEntry.Create(
                notification.Item.Id,
                from,
                to,
                from.Contract,
                to.Contract,
                now);

            await unitOfWork.DbContext.ParticipantOutgoingTransferQueue.AddAsync(outgoingTransfer, cancellationToken);
        }

        // Only publish incoming transfer if (destination/target) contract exists
        if (to is { Contract: not null })
        {
            var incomingTransfer = ParticipantIncomingTransferQueueEntry.Create(
                notification.Item.Id,
                from,
                to,
                from.Contract,
                to.Contract,
                now);

            await unitOfWork.DbContext.ParticipantIncomingTransferQueue.AddAsync(incomingTransfer, cancellationToken);
        }
    }
}
