﻿using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyOwnerParticipantHasBeenReturnedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.From == EnrolmentStatus.SubmittedToProviderStatus && notification.To == EnrolmentStatus.EnrollingStatus)
        {
            const string heading = "Enrolment returned";

            string details = "You have enrolments that have been returned by PQA";

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                && n.OwnerId == notification.Item.OwnerId
                && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if(previous is null)
            {
                var n = Notification.Create(heading, details, notification.Item.OwnerId!);
                n.SetLink($"/pages/participants/?listView=Enrolling");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}


