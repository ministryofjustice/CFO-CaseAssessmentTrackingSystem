using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers
{
    public class CreateActivityQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
        : INotificationHandler<ActivityQa1EntryCompletedDomainEvent>
    {
        public async Task Handle(ActivityQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var queueEntry = ActivityQa2QueueEntry.Create(notification.Entry.Id);
            queueEntry.TenantId = notification.Entry!.Activity!.TenantId!;
            queueEntry.Participant = notification.Entry!.Participant;
            
            await unitOfWork.DbContext.ActivityQa2Queue.AddAsync(queueEntry, cancellationToken);
        }
    }
}