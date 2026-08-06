using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyPqaActivityRejectedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityPqaQueueCreatedDomainEvent>
{
    public async Task Handle(ActivityPqaQueueCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Only notify if this is a REJECTED entry (returned from authority)
        if (notification.Entity.IsAccepted == false)
        {
            // Get activity details for the notification message
            var activity = await unitOfWork.DbContext
                .Activities.AsNoTracking()
                .Where(a => a.Id == notification.Entity.ActivityId)
                .Select(a => new { a.Definition.Name, a.ParticipantId })
                .FirstOrDefaultAsync(cancellationToken);
            
            if (activity == null)
            {
                return;
            }

            var heading = $"Activity returned - {activity.ParticipantId}";
            var details = $"{activity.Name} activity has been returned from the authority for review";

            // Get the PREVIOUS accepted PQA queue entry to find who originally did the PQA
            var qaUser = await unitOfWork.DbContext
                .ActivityPqaQueue.AsNoTracking()
                .Where(pqa =>
                    pqa.ActivityId == notification.Entity.ActivityId 
                    && pqa.IsAccepted == true 
                    && pqa.IsCompleted == true)
                .Select(pqa => pqa.LastModifiedBy)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (qaUser == null)
            {
                return;
            }

            // Use the entity ID directly from the event for the link
            var link = $"pages/workspace/deliverymanagement/activities/pqa/{notification.Entity.Id}";

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