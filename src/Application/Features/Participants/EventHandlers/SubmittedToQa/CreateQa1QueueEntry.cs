using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class CreateQa1QueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{

    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToAuthorityStatus)
        {
            var queueEntry = EnrolmentQa1QueueEntry.Create(notification.Item.Id);
            queueEntry.TenantId = notification.Item.Owner!.TenantId!;
            await unitOfWork.DbContext.EnrolmentQa1Queue.AddAsync(queueEntry, cancellationToken);    
        }
    }
}