using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.SubmittedToPqa;

public class CreateActivityPqaEntryFromSubmittedToAuthority(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {

        if (notification.To == ActivityStatus.SubmittedToProviderStatus &&
               notification.From == ActivityStatus.SubmittedToAuthorityStatus)
        {            
            if (notification.Item.Owner is null)
            {
                throw new ApplicationException("Owner must be set");
            }

            if (notification.Item.Owner.TenantId is null)
            {
                throw new ApplicationException("Owner tenant id must be set");
            }

            //get the values from the LAST PQA
            var lastPqa = await unitOfWork
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

            var queueEntry = new ActivityPqaQueueEntry(notification.Item.Id, lastPqa.TenantId, lastPqa.SupportWorkerId, lastPqa.OriginalPQASubmissionDate,lastPqa.ParticipantId!);

            await unitOfWork.DbContext.ActivityPqaQueue.AddAsync(queueEntry, cancellationToken);
        }
    }
}