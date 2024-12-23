﻿using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class ActivityApproved(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>

{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == ActivityStatus.ApprovedStatus && notification.Item.RequiresQa == true)
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
                n.SetLink($"/pages/activities/?listView=Approved");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }

        await Task.CompletedTask;
    }
}