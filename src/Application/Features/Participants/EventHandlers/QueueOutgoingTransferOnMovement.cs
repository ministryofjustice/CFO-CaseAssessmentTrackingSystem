using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class QueueOutgoingTransferOnMovement(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
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

        // Find any outgoing transfers which have now become stale. These tranfers are either:
        // Leaving a contract they have left in the past
        // OR
        // Returning to a contract they have left in the past
        var staleOutgoingTransfers = await unitOfWork.DbContext.ParticipantOutgoingTransferQueue
            .Where(q => q.IsReplaced == false)
            .Where(q => q.ParticipantId == notification.Item.Id)
            //.Include(q => q.FromContract)
            .Where(q => q.FromContract == from.Contract || q.FromContract == to.Contract)
            .ToListAsync(cancellationToken);

        foreach(var transfer in staleOutgoingTransfers)
        {
            transfer.MarkAsReplaced();
        }

        // Only publish an outgoing transfer if:
        // The source/original contract exists
        // AND...
        // This is a cross-contract transfer
        if (from.Contract is not null && from.Contract != to.Contract)
        {
            var outgoingTransfer = ParticipantOutgoingTransferQueueEntry.Create(
                notification.Item.Id,
                from,
                to,
                from.Contract,
                to.Contract,
                DateTime.UtcNow);

            await unitOfWork.DbContext.ParticipantOutgoingTransferQueue.AddAsync(outgoingTransfer, cancellationToken);
        }
    }
}