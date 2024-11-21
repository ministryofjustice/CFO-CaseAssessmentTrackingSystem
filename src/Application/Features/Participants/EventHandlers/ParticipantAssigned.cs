using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantAssigned(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var previous = await unitOfWork.DbContext.ParticipantOwnershipHistories
            .FirstOrDefaultAsync(x => x.ParticipantId == notification.Item.Id && x.To == null, cancellationToken);

        previous?.SetTo(now);

        var owner = await unitOfWork.DbContext.Users.FindAsync([notification.NewOwner], cancellationToken);

        var history = ParticipantOwnershipHistory.Create(
            notification.Item.Id,
            owner?.Id,
            owner?.TenantId,
            now);

        await unitOfWork.DbContext.ParticipantOwnershipHistories.AddAsync(history, cancellationToken);
    }

}
