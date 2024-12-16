using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers
{
    public class CreateActivityQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
        : INotificationHandler<ActivityQa1EntryCompletedDomainEvent>
    {
        public async Task Handle(ActivityQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var entry = ActivityQa2QueueEntry.Create(notification.Entry.Id);
            entry.TenantId = notification.Entry!.Participant!.Owner!.TenantId!;
            await unitOfWork.DbContext.ActivityQa2Queue.AddAsync(entry, cancellationToken);
        }
    }
}