using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.QualityAssurance;

public class CreateActivityQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<ActivityQa1EntryCompletedDomainEvent>
{
    public async Task Handle(ActivityQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {            
        // get the most recent PQA entry
        var pqa = await unitOfWork
            .DbContext.ActivityPqaQueue
            .AsNoTracking()
            .Where(q => q.ActivityId == notification.Entry.ActivityId)
            .OrderByDescending(q => q.Created)
            .Take(1)
            .Select(q => new
            {
                q.TenantId,
                q.SupportWorkerId,
                q.OriginalPQASubmissionDate,
                q.ParticipantId
            })
            .FirstAsync(cancellationToken);

        var qa2 = new ActivityQa2QueueEntry(notification.Entry.ActivityId, pqa.TenantId, pqa.SupportWorkerId, pqa.OriginalPQASubmissionDate, pqa.ParticipantId!);

        await unitOfWork.DbContext.ActivityQa2Queue.AddAsync(qa2, cancellationToken);
    }
}