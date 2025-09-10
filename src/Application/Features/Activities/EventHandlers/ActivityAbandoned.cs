using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class ActivityAbandoned : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {        
        return Task.CompletedTask;
    }
}
