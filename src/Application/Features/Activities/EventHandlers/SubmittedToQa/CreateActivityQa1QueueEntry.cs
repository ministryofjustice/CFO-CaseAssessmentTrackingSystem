using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.SubmittedToQa
{
    public class CreateActivityQa1QueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
    {
        public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.To == ActivityStatus.SubmittedToAuthorityStatus)
            {
                var queueEntry = ActivityQa1QueueEntry.Create(notification.Item.Id);
                queueEntry.TenantId = notification.Item.TenantId!;
                queueEntry.ParticipantId = notification.Item.ParticipantId;

                await unitOfWork.DbContext.ActivityQa1Queue.AddAsync(queueEntry, cancellationToken);
            }
        }
    }
}