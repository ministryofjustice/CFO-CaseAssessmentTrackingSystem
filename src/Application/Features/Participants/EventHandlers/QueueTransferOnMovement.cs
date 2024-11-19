﻿using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class QueueTransferOnMovement(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        string participantId = notification.Item.Id;

        var locations = await unitOfWork.DbContext.Locations
            .Where(l => new[] { notification.From.Id, notification.To.Id }.Contains(l.Id))
            .Include(l => l.Contract)
            .ToDictionaryAsync(l => l.Id, cancellationToken);

        var (from, to) = (locations[notification.From.Id], locations[notification.To.Id]);

        await CloseStaleOutgoingTransfers(participantId, from, to, cancellationToken);

        var latestIncomingTransfer = await unitOfWork.DbContext.ParticipantIncomingTransferQueue
            .Where(q => q.ParticipantId == participantId)
            .OrderByDescending(q => q.Created)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestIncomingTransfer is { Completed: true })
        {
            await PublishOutgoingTransfer(participantId, from, to, cancellationToken);
        }
        else
        {
            latestIncomingTransfer?.Complete();
        }

        await PublishIncomingTransfer(participantId, from, to, cancellationToken);

    }

    async Task PublishOutgoingTransfer(string participantId, Location from, Location to, CancellationToken cancellationToken)
    {
        // Only publish an outgoing transfer if:
        // The source/original contract exists
        // AND...
        // This is a cross-contract transfer
        if (from.Contract is not null && from.Contract != to.Contract)
        {
            var outgoingTransfer = ParticipantOutgoingTransferQueueEntry.Create(
                participantId,
                from,
                to,
                from.Contract,
                to.Contract,
                DateTime.UtcNow);

            await unitOfWork.DbContext.ParticipantOutgoingTransferQueue.AddAsync(outgoingTransfer, cancellationToken);
        }
    }

    async Task PublishIncomingTransfer(string participantId, Location from, Location to, CancellationToken cancellationToken)
    {
        // Only publish incoming transfer if (destination/target) contract exists
        if (to.Contract is not null)
        {
            var incomingTransfer = ParticipantIncomingTransferQueueEntry.Create(
                participantId,
                from,
                to,
                from.Contract,
                to.Contract,
                DateTime.UtcNow);

            await unitOfWork.DbContext.ParticipantIncomingTransferQueue.AddAsync(incomingTransfer, cancellationToken);
        }
    }

    async Task CloseStaleOutgoingTransfers(string participantId, Location from, Location to, CancellationToken cancellationToken)
    {
        // Find any outgoing transfers which have now become stale. These tranfers are either:
        // Leaving a contract they have left in the past
        // OR
        // Returning to a contract they have left in the past
        var staleOutgoingTransfers = await unitOfWork.DbContext.ParticipantOutgoingTransferQueue
            .Where(q => q.IsReplaced == false)
            .Where(q => q.ParticipantId == participantId)
            .Where(q => q.FromContract == from.Contract || q.FromContract == to.Contract)
            .ToListAsync(cancellationToken);

        foreach (var transfer in staleOutgoingTransfers)
        {
            transfer.MarkAsReplaced();
        }
    }

}
