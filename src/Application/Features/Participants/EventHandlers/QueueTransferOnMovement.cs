using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class QueueTransferOnMovement(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Only publish outgoing transfer if (source/original) contract exists
        if (notification.From.Contract is not null)
        {
            var outgoingTransfer = ParticipantOutgoingTransferQueueEntry.Create(
                notification.Item.Id,
                notification.From,
                notification.To,
                notification.From.Contract,
                notification.To.Contract,
                now);

            await unitOfWork.DbContext.ParticipantOutgoingTransferQueue.AddAsync(outgoingTransfer, cancellationToken);
        }

        // Only publish incoming transfer if (destination/target) contract exists
        if (notification.To.Contract is not null)
        {
            var incomingTransfer = ParticipantIncomingTransferQueueEntry.Create(
                notification.Item.Id,
                notification.From,
                notification.To,
                notification.From.Contract,
                notification.To.Contract,
                now);

            await unitOfWork.DbContext.ParticipantIncomingTransferQueue.AddAsync(incomingTransfer, cancellationToken);
        }
    }
}
