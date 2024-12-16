using Cfo.Cats.Domain.Entities.Payables;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers
{
    internal class ActivityQa2EscalatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityQa2EntryEscalatedDomainEvent>
    {
        public async Task Handle(ActivityQa2EntryEscalatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var entry = ActivityEscalationQueueEntry.Create(notification.Entry.Id);
            entry.TenantId = notification.Entry!.Participant!.Owner!.TenantId!;
            await unitOfWork.DbContext.ActivityEscalationQueue.AddAsync(entry, cancellationToken);
        }
    }
}