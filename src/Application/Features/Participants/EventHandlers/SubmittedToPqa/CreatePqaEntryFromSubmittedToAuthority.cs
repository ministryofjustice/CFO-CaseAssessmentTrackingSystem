using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToPqa;

public class CreatePqaEntryFromSubmittedToAuthority(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToProviderStatus && notification.From == EnrolmentStatus.SubmittedToAuthorityStatus)
        {
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
            
            var queueEntry = EnrolmentPqaQueueEntry.Create(notification.Item.Id, lastPqa.SupportWorkerId, lastPqa.ConsentDate);
            queueEntry.TenantId = lastPqa.TenantId;
            await unitOfWork.DbContext.EnrolmentPqaQueue.AddAsync(queueEntry, cancellationToken);
        }
    }
}