using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.SubmittedToPqa;

public class CreateActivityPqaQueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == ActivityStatus.SubmittedToProviderStatus &&
            notification.From == ActivityStatus.PendingStatus)
        {
            if (notification.Item.Owner is null)
            {
                throw new ApplicationException("Owner must be set");
            }

            if (notification.Item.Owner.TenantId is null)
            {
                throw new ApplicationException("Owner tenant id must be set");
            }

            if (notification.Item.ParticipantId is null)
            {
                throw new ApplicationException("Participant id must be set");
            }

            var queueEntry = new ActivityPqaQueueEntry(notification.Item.Id, notification.Item.Owner.TenantId,
                notification.Item.Owner.Id, DateTime.UtcNow, notification.Item.ParticipantId);
                            
            await unitOfWork.DbContext.ActivityPqaQueue.AddAsync(queueEntry, cancellationToken);             
        }
    }
}