using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.QualityAssurance;

internal class ActivityQa2EscalatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityQa2EntryEscalatedDomainEvent>
{
    public async Task Handle(ActivityQa2EntryEscalatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var entry = new ActivityEscalationQueueEntry(notification.Entry.ActivityId, notification.Entry.TenantId, notification.Entry.SupportWorkerId, notification.Entry.OriginalPQASubmissionDate, notification.Entry.ParticipantId!);
        await unitOfWork.DbContext.ActivityEscalationQueue.AddAsync(entry, cancellationToken);
    }
}