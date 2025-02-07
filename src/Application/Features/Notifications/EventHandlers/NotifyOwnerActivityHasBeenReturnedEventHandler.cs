using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyOwnerActivityHasBeenReturnedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Item.OwnerId is null)
        {
            return;
        }

        if (notification.From == ActivityStatus.SubmittedToProviderStatus && notification.To == ActivityStatus.PendingStatus)
        {
            const string heading = "Activity returned";

            string details = "You have activities that have been returned by PQA";

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                && n.OwnerId == notification.Item.OwnerId
                && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if(previous is null)
            {
                var n = Notification.Create(heading, details, notification.Item.OwnerId);
                n.SetLink($"/pages/participants/" + notification.Item.ParticipantId);                
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}



