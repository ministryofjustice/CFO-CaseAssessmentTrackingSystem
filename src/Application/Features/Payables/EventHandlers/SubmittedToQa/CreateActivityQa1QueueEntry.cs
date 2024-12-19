using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers.SubmittedToQa
{
    public class CreateActivityQa1QueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
    {
        public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.To == ActivityStatus.SubmittedToAuthorityStatus)
            {
                var queueEntry = ActivityQa1QueueEntry.Create(notification.Item.Id);
                queueEntry.TenantId = notification.Item.Participant!.Owner!.TenantId!;
                queueEntry.Participant!.Id = notification.Item.Participant!.Id;

                await unitOfWork.DbContext.ActivityQa1Queue.AddAsync(queueEntry, cancellationToken);
            }
        }
    }
}