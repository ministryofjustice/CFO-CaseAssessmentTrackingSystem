using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantAssigned(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var owner = await unitOfWork.DbContext.Users.FindAsync(notification.NewOwner);

        if(owner is null)
        {
            return;
        }

        var history = ParticipantOwnershipHistory.Create(
            notification.Item.Id,
            owner.Id,
            owner.TenantId!,
            DateTime.UtcNow);

        await unitOfWork.DbContext.ParticipantOwnershipHistories.AddAsync(history, cancellationToken);
    }

}
