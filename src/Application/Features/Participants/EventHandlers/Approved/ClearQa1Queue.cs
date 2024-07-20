using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.Approved;

public class ClearQa1Queue(IUnitOfWork unitOfWork): INotificationHandler<EnrolmentQa2QueueCreatedDomainEvent>
{

    public async Task Handle(EnrolmentQa2QueueCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        // remove all QA2
        var events = unitOfWork.DbContext.EnrolmentQa1Queue
            .Where(e => e.ParticipantId == notification.Entity.ParticipantId)
            .Where(e => e.IsCompleted == false);

        foreach (var e in events)
        {
            // should only be one.
            e.Complete();
        }

    }
}