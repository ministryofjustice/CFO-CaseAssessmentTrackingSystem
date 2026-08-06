using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Entities.PRIs;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyCommunitySupportWorkerPriAssignedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<PRIAssignedDomainEvent>
{
    public async Task Handle(PRIAssignedDomainEvent pri, CancellationToken cancellationToken)
    {
        if (pri.Item is { AssignedTo.Length: > 0 } && pri.Item.CreatedBy != pri.Item.AssignedTo)
        {
            const string heading = "PRI assigned";

            string details = "You have been assigned a PRI";

            var n = Notification.Create(heading, details, pri.Item.AssignedTo);
            n.SetLink($"/pages/workspace/participants/{pri.Item.ParticipantId}/");
            await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
        }
    }
}