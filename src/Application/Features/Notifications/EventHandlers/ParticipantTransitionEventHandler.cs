using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class ParticipantTransitionEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.ApprovedStatus)
        {
            string heading = "Enrolment Approved";
            string details = $"Enrolment for {notification.Item.FirstName} {notification.Item.LastName} has been approved.";
            var n = Notification.Create(heading, details, notification.Item.OwnerId!);
            await unitOfWork.DbContext.Notifications.AddAsync(n);
        }
    }
}