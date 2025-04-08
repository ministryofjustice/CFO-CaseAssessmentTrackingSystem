using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToPqa;

public class CreatePqaEntryFromEnrolling(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public  async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.SubmittedToProviderStatus &&
            notification.From == EnrolmentStatus.EnrollingStatus)
        {
            if (notification.Item.Owner is null)
            {
                throw new ApplicationException("Owner must be set");
            }

            if (notification.Item.Owner.TenantId is null)
            {
                throw new ApplicationException("Owner tenant id must be set");
            }

            var queueEntry = new EnrolmentPqaQueueEntry(notification.Item.Id, notification.Item.Owner.TenantId,
                notification.Item.Owner.Id, notification.Item.CalculateConsentDate()!.Value);
  
            await unitOfWork.DbContext.EnrolmentPqaQueue.AddAsync(queueEntry, cancellationToken);
        }
    }
}