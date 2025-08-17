using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToQa;

public class CreateQa1QueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{

    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToAuthorityStatus)
        {
            // get the most recent PQA entry
            var pqa = await unitOfWork
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

            var qa1 = new EnrolmentQa1QueueEntry(notification.Item.Id, pqa.TenantId, pqa.SupportWorkerId,
                pqa.ConsentDate);

            await unitOfWork.DbContext.EnrolmentQa1Queue.AddAsync(qa1, cancellationToken);    
        }
    }
}