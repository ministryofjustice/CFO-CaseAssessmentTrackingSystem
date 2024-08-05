using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToPqa;

public class CreateEnrolmentPqaQueueEntry(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public  async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToProviderStatus)
        {
            var queueEntry = EnrolmentPqaQueueEntry.Create(notification.Item.Id);
            queueEntry.TenantId = notification.Item.Owner!.TenantId!;
            await unitOfWork.DbContext.EnrolmentPqaQueue.AddAsync(queueEntry, cancellationToken);    
        }
    }
}
