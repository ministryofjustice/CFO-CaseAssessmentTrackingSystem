using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class QueueTransferOnMovement(IUnitOfWork unitOfWork,        
      ILocationService locationService) //: INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var createOutGoingQueue = false;

        //Get all items off Incoming Queue for the participant
        var allIncomingQueueEntriesForParticipant = await unitOfWork.DbContext.ParticipantIncomingTransferQueue
            .Where(q => q.ParticipantId == notification.Item.Id)
            .ToListAsync(cancellationToken);

        //Are there any items on queue ?
        if (allIncomingQueueEntriesForParticipant.Any())
        {
            //Are there already open items on queue for participant ?
            var openItem = allIncomingQueueEntriesForParticipant?.Where(q => q.Completed == false).FirstOrDefault();
            if (openItem != null)
            {
                //get list of owners
                var previouslyOwned = await unitOfWork.DbContext.ParticipantOwnershipHistories
                    .Where(x => x.ParticipantId == notification.Item.Id
                    && x.From > DateTime.UtcNow.AddDays(-60))
                    //.Where(x => x.TenantId.StartsWith(notification.From.Tenants))
                    .ToListAsync(cancellationToken);               

                //are there previous owners
                if (previouslyOwned.Any())
                {
                    foreach (var previousOwer in previouslyOwned)
                    {
                        var ownedLocations = locationService.GetVisibleLocations(previousOwer.TenantId).Select(location => location.Id);

                        //Dont add to Outgoing queue unless particiapnt has been assigned to them in last 3 months. (ownership table)
                        if (ownedLocations.Any())
                        {
                            createOutGoingQueue = true;
                        }
                    }
                }

                //close open item on queue                
                openItem?.Complete();
            }
            //inputqueue item on queue but been closed / assigned
            else
            {
                createOutGoingQueue = true;
            }
        }
        //Effectively first time transferred.
        else
        {
            createOutGoingQueue = true;
        }

        var locations = await unitOfWork.DbContext.Locations
            .Where(l => new[] { notification.From.Id, notification.To.Id }.Contains(l.Id))
            .Include(l => l.Contract)
            .ToListAsync(cancellationToken);

        var from = locations.First(l => l.Id == notification.From.Id);
        var to = locations.First(l => l.Id == notification.To.Id);

        if (createOutGoingQueue)
        {
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