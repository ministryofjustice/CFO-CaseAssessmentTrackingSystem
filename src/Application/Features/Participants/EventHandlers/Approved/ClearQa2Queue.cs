using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.Approved;

public class ClearQa2Queue(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == EnrolmentStatus.ApprovedStatus)
        {
            await Task.CompletedTask;

            // remove all QA2
            var events = unitOfWork.DbContext.EnrolmentQa2Queue
                .Where(e => e.ParticipantId == notification.Item.Id)
                .Where(e => e.IsCompleted == false);

            foreach (var e in events)
            {
                // should only be one.
                e.Complete();
            }
        }
    }
}
