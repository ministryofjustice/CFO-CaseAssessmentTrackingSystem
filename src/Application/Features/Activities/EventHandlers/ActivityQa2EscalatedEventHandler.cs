using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers
{
    internal class ActivityQa2EscalatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityQa2EntryEscalatedDomainEvent>
    {
        public async Task Handle(ActivityQa2EntryEscalatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var entry = ActivityEscalationQueueEntry.Create(notification.Entry.Id);
            entry.TenantId = notification.Entry!.Activity!.TenantId!;
            await unitOfWork.DbContext.ActivityEscalationQueue.AddAsync(entry, cancellationToken);
        }
    }
}