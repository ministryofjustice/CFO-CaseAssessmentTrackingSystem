using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class EnrolmentQueueEntryCompletedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<EnrolmentQueueEntryCompletedDomainEvent>
{
    public async Task Handle(EnrolmentQueueEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry is EnrolmentQa1QueueEntry)
        {
            var entry = EnrolmentQa2QueueEntry.Create(notification.Entry.ParticipantId);
            entry.TenantId = notification.Entry!.Participant!.Owner!.TenantId!;
            await unitOfWork.DbContext.EnrolmentQa2Queue.AddAsync(entry, cancellationToken);
        }
        
        if (notification.Entry is EnrolmentQa2QueueEntry)
        {
            if (notification.Entry.IsAccepted)
            {
                notification.Entry.Participant!.TransitionTo(EnrolmentStatus.ApprovedStatus)
                    .ApproveConsent();
            }
            else
            {
                notification.Entry.Participant!.TransitionTo(EnrolmentStatus.SubmittedToProviderStatus);
            }
        }
        
        
        
        
    }
}