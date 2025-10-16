using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RecordParticipantUnassigned(IUnitOfWork unitOfWork, IDateTime dateTime) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.NewOwner is not null)
        {
            // there is a new owner, nothing to do.
            // the other handler will take care of it.
            return;
        }

        var newContractId = await unitOfWork.DbContext.Locations
                                .Where(l => l.Id == notification.CurrentLocationId)
                                #pragma warning disable CS8602 
                                .Select(l => l.Contract.Id)
                                #pragma warning restore CS8602 
                                .FirstOrDefaultAsync(cancellationToken);

        var now = dateTime.Now;

        var history = ParticipantOwnershipHistory.Create(
            notification.Item.Id,
            null,
            null,
            newContractId,
            now);

        var previousOwner = await unitOfWork.DbContext
           .ParticipantOwnershipHistories
           .FirstOrDefaultAsync(x => x.ParticipantId == notification.Item.Id && x.To == null, cancellationToken);

        if (previousOwner?.OwnerId == history.OwnerId && previousOwner?.TenantId == history.TenantId && previousOwner?.ContractId == history.ContractId)
        {
            // no change return (possible nothing has changed)
            return;
        }

        previousOwner?.SetTo(now);
        await unitOfWork.DbContext.ParticipantOwnershipHistories.AddAsync(history, cancellationToken);

    }
}