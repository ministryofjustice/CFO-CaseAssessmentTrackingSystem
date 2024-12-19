using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers.SubmittedToQa
{
    public class CreateActivityPqaQueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
    {
        public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.To == ActivityStatus.SubmittedToProviderStatus)
            {
                var queueEntry = ActivityPqaQueueEntry.Create(notification.Item.Id, notification.Item.Participant!.Id);                
                queueEntry.TenantId = notification.Item.Participant!.Owner!.TenantId!;
                
                await unitOfWork.DbContext.ActivityPqaQueue.AddAsync(queueEntry, cancellationToken);
            }
        }
    }
}