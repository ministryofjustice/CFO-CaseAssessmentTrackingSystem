using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers;

public class ActivityCreated : INotificationHandler<ActivityCreatedDomainEvent>
{
    public Task Handle(ActivityCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.Entity.RequiresQa)
        {
            // Add to Queue
        }
        else
        {
            notification.Entity.TransitionTo(ActivityStatus.ApprovedStatus);
        }

        return Task.CompletedTask;
    }
}
