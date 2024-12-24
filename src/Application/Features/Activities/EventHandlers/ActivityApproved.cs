using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers;

public class ActivityApproved : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.Item.Status == ActivityStatus.ApprovedStatus)
        {
            notification.Item.Approve();
        }

        return Task.CompletedTask;
    }
}
