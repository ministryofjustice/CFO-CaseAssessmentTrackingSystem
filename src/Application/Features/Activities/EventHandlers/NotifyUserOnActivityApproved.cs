using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class NotifyUserOnActivityApproved(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == ActivityStatus.ApprovedStatus && notification.Item.RequiresQa)
        {
            const string heading = "Activity approved";
            const string details = "You have activities that have been approved";

            var previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                && n.OwnerId == notification.Item.OwnerId
                && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if (previous is null)
            {
                var n = Notification.Create(heading, details, notification.Item.OwnerId!);
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}