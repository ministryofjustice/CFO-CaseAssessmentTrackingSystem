using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyOwnerParticipantHasBeenReturnedEventHandler(IUnitOfWork unitOfWork, ILogger<NotifyOwnerParticipantHasBeenReturnedEventHandler> logger) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.From == EnrolmentStatus.SubmittedToProviderStatus && notification.To == EnrolmentStatus.EnrollingStatus)
        {
            if (notification.Item.OwnerId is null)
            {
                logger.LogDebug("Participant {ParticipantId} without an owner has been returned. Notification ignored.", notification.Item.Id);
                return;
            }

            const string heading = "Enrolment returned";

            string details = $"Your enrolment for {notification.Item.FullName} at {notification.Item.EnrolmentLocation?.Name} has been returned.";
          
            var n = Notification.Create(heading, details, notification.Item.OwnerId!);
            n.SetLink($"/pages/workspace/participants/{notification.Item.Id}/");
            await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
        }
    }
}

