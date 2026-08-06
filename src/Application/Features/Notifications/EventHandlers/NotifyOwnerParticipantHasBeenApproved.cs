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
                logger.LogDebug("Participant {ParticipantId} without an owner has been approved. Notification ignored.", notification.Item.Id);
                return;
            }

            const string heading = "Enrolment approved";

            string details = $"Your enrolment for {notification.Item.FullName} at {notification.Item.EnrolmentLocation?.Name} has been approved.";
   
            var n = Notification.Create(heading, details, notification.Item.OwnerId!);
            n.SetLink($"/pages/workspace/participants/{notification.Item.Id}/");
            await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
    }
    }
}