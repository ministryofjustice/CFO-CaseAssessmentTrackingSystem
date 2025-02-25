using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToPqa;

public class CreatePqaEntryFromSubmittedToAuthority(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToProviderStatus && notification.From == EnrolmentStatus.SubmittedToAuthorityStatus)
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
                .DbContext.EnrolmentPqaQueue
                .AsNoTracking()
                .Where(q => q.ParticipantId == notification.Item.Id)
                .OrderByDescending(q => q.Created)
                .Take(1)
                .Select(q => new
                {
                    q.TenantId,
                    q.SupportWorkerId,
                    q.ConsentDate
                })
                .FirstAsync(cancellationToken);
            
            var queueEntry = new EnrolmentPqaQueueEntry(notification.Item.Id, lastPqa.TenantId ,lastPqa.SupportWorkerId, lastPqa.ConsentDate);

            await unitOfWork.DbContext.EnrolmentPqaQueue.AddAsync(queueEntry, cancellationToken);
        }
    }
}