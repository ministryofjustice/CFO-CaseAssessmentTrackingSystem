using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class PublishActivityAbandonedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == ActivityStatus.AbandonedStatus)
        {
            await unitOfWork.DbContext.InsertOutboxMessage(new ActivityAbandonedIntegrationEvent(notification.Item, notification.DateOccurred.DateTime));
        }
    }
}