using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PRIs.EventHandlers;

public class ClearCodesOnAssignment(IUnitOfWork unitOfWork) : INotificationHandler<PRIAssignedDomainEvent>
{
    public async Task Handle(PRIAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.PriCodes
            .Where(p => p.ParticipantId == notification.Item.ParticipantId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
