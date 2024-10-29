using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyOwnerParticipantHasBeenApproved(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.ApprovedStatus)
        {
            const string heading = "Enrolment approved";

            string details = "You have enrolments that have been approved";

            if (unitOfWork.DbContext.Notifications.Any(n =>
                    n.Heading == heading
                    && n.OwnerId == notification.Item.OwnerId
                    && n.ReadDate == null
                ) == false)
            {
                var n = Notification.Create(heading, details, notification.Item.OwnerId!);
                n.SetLink($"/pages/participants/?listView=Approved");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}