using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyOwnerActivityHasBeenReturnedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.From == ActivityStatus.SubmittedToProviderStatus && notification.To == ActivityStatus.PendingStatus)
        {
            const string heading = "Activity returned";

            string details = $"Your {notification.Item.Type.Name} ({notification.Item.Category.Name}) activity has been returned by QA.";

            var n = Notification.Create(heading, details, notification.Item.OwnerId!);
            n.SetLink($"/pages/workspace/participants/" + notification.Item.ParticipantId);                
            await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
        }
    }
}

