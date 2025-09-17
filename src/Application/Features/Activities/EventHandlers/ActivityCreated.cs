using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

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
            //Auto approve if no QA required
            notification.Entity.Approve(null);                
        }

        return Task.CompletedTask;
    }
}
