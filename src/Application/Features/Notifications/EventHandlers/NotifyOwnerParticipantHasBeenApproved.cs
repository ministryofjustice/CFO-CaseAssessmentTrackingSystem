using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyOwnerParticipantHasBeenApproved(IUnitOfWork unitOfWork, ILogger<NotifyOwnerParticipantHasBeenApproved> logger) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.ApprovedStatus)
        {
            if (notification.Item.OwnerId is null)
            {
                logger.LogWarning("Participant {ParticipantId} without an owner has been approved. Notification ignored.", notification.Item.Id);
                return;
            }

            const string heading = "Enrolment approved";

            string details = "You have enrolments that have been approved";

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                && n.OwnerId == notification.Item.OwnerId
                && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if (previous is null)
            {
                var n = Notification.Create(heading, details, notification.Item.OwnerId!);
                n.SetLink($"/pages/participants/?listView=Approved");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
           
        }
    }
}