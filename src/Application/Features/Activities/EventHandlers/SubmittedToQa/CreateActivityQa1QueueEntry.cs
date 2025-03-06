using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.SubmittedToQa;

public class CreateActivityQa1QueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == ActivityStatus.SubmittedToAuthorityStatus)
        {
            // get the most recent PQA entry
            var pqa = await unitOfWork
                .DbContext.ActivityPqaQueue
                .AsNoTracking()
                .Where(q => q.ActivityId == notification.Item.Id)
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

            var qa1 = new ActivityQa1QueueEntry(notification.Item.Id, pqa.TenantId, pqa.SupportWorkerId, pqa.OriginalPQASubmissionDate,pqa.ParticipantId!);

            await unitOfWork.DbContext.ActivityQa1Queue.AddAsync(qa1, cancellationToken);
        }
    }
}