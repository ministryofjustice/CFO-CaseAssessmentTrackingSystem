using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers;

public class ActivityApproved : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.To == ActivityStatus.ApprovedStatus)
        {
            // Do some stuff
        }

        await Task.CompletedTask;
    }
}
