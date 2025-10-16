using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

/// <summary>
/// Handles assignments (or reassigments) for participant history
/// </summary>
/// <param name="unitOfWork"></param>
public class RecordParticipantAssignment(IUnitOfWork unitOfWork, IDateTime dateTime) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.NewOwner is null)
        {
            // no new owner, nothing to do.
            // the other handler will take care of it.
            return;
        }

        var now = dateTime.Now;

        string newTenantId = await unitOfWork.DbContext.Users
                                .Where(u => u.Id == notification.NewOwner)
                                .Select(u => u.TenantId!)
                                .FirstAsync(cancellationToken);

        string? newContractId = await unitOfWork.DbContext.Tenants
                                .Where(t => t.Id == newTenantId)
                                .Select(t => t.ContractId)
                                .FirstOrDefaultAsync(cancellationToken);

        if(newContractId is null)
        {
            newContractId = await unitOfWork.DbContext.Locations
                                .Where(l => l.Id == notification.CurrentLocationId)
                                #pragma warning disable CS8602 
                                .Select(l => l.Contract.Id)
                                #pragma warning restore CS8602 
                                .FirstOrDefaultAsync(cancellationToken);
        }

        var previousOwner = await unitOfWork.DbContext
            .ParticipantOwnershipHistories
            .FirstOrDefaultAsync(x => x.ParticipantId == notification.Item.Id && x.To == null, cancellationToken);

        var history = ParticipantOwnershipHistory.Create(
            notification.Item.Id,
            notification.NewOwner,
            newTenantId,
            newContractId,
            now);

        // have we changed?
        if (previousOwner?.OwnerId == history.OwnerId && previousOwner?.TenantId == history.TenantId && previousOwner?.ContractId == history.ContractId)
        {
            // no change return (possible nothing has changed)
            return;
        }

        previousOwner?.SetTo(now);
        await unitOfWork.DbContext.ParticipantOwnershipHistories.AddAsync(history, cancellationToken);
    }

}
