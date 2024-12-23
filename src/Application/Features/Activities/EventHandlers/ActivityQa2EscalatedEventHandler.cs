using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers
{
    internal class ActivityQa2EscalatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityQa2EntryEscalatedDomainEvent>
    {
        public async Task Handle(ActivityQa2EntryEscalatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var entry = ActivityEscalationQueueEntry.Create(notification.Entry.ActivityId, notification.Entry!.Activity!.TenantId!, notification.Entry!.Activity!.Participant!);
            await unitOfWork.DbContext.ActivityEscalationQueue.AddAsync(entry, cancellationToken);
        }
    }
}