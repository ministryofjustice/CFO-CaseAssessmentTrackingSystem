using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.SubmittedToQa
{
    public class CreateActivityPqaQueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
    {
        public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.To == ActivityStatus.SubmittedToProviderStatus)
            {
                var queueEntry = ActivityPqaQueueEntry.Create(notification.Item.Id);                
                queueEntry.TenantId = notification.Item.TenantId!;
                queueEntry.Participant=notification.Item.Participant;

                await unitOfWork.DbContext.ActivityPqaQueue.AddAsync(queueEntry, cancellationToken);
            }
        }
    }
}