using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers;

public class ActivityCreated : INotificationHandler<ActivityCreatedDomainEvent>
{
    public Task Handle(ActivityCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.Entity.RequiresQa)
        {
            notification.Entity.TransitionTo(ActivityStatus.SubmittedToProviderStatus);
        }
        else
        {
            notification.Entity.TransitionTo(ActivityStatus.ApprovedStatus);
        }

        return Task.CompletedTask;
    }
}
