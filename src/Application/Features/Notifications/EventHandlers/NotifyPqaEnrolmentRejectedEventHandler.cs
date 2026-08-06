using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyPqaEnrolmentRejectedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<EnrolmentPqaQueueCreatedDomainEvent>
{
    public async Task Handle(EnrolmentPqaQueueCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Only notify if this is a REJECTED entry (returned from authority)
        if (notification.Entity.IsAccepted == false)
        {
            var heading = $"Enrolment returned - {notification.Entity.ParticipantId}";
            var details = "This enrolment has been returned from the authority for review";

            // Get the PREVIOUS accepted PQA queue entry to find who originally did the PQA
            var qaUser = await unitOfWork.DbContext
                .EnrolmentPqaQueue.AsNoTracking()
                .Where(pqa =>
                    pqa.ParticipantId == notification.Entity.ParticipantId 
                    && pqa.IsAccepted == true 
                    && pqa.IsCompleted == true)
                .Select(pqa => pqa.LastModifiedBy)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (qaUser == null)
            {
                return;
            }

            // Use the entity ID directly from the event for the link
            var link = $"pages/workspace/deliverymanagement/enrolments/pqa/{notification.Entity.Id}";

            var previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                    && n.OwnerId == qaUser
                    && n.Link == link
                    && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if (previous == null)
            {
                var n = Notification.Create(heading, details, qaUser);
                n.SetLink(link);
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}