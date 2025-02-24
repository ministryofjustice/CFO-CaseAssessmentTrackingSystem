using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToPqa;

public class CreatePqaEntryFromEnrolling(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public  async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToProviderStatus && notification.From == EnrolmentStatus.EnrollingStatus)
        {
            var queueEntry = EnrolmentPqaQueueEntry.Create(notification.Item.Id, notification.Item.OwnerId!, notification.Item.Consents.MaxBy(c => c.Created)!.Lifetime.StartDate);
            queueEntry.TenantId = notification.Item.Owner!.TenantId!;
            await unitOfWork.DbContext.EnrolmentPqaQueue.AddAsync(queueEntry, cancellationToken);    
        }
    }
}