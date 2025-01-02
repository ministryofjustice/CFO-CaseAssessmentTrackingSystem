using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers
{
    public class CreateActivityQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
        : INotificationHandler<ActivityQa1EntryCompletedDomainEvent>
    {
        public async Task Handle(ActivityQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var queueEntry = ActivityQa2QueueEntry.Create(notification.Entry.ActivityId, notification.Entry!.Activity!.TenantId!, notification.Entry!.Participant!);
            await unitOfWork.DbContext.ActivityQa2Queue.AddAsync(queueEntry, cancellationToken);
        }
    }
}