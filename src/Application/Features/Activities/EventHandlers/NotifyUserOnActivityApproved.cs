using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class NotifyUserOnActivityApproved(IUnitOfWork unitOfWork) : INotificationHandler<ActivityApprovedDomainEvent>

{
    public async Task Handle(ActivityApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Item.RequiresQa == true)
        {
            // Do some stuff        
            const string heading = "Activity approved";

            string details = "You have activities that have been approved";

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
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

        await Task.CompletedTask;
    }
}