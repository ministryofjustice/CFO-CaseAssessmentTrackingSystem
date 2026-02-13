using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class PublishActivityApprovedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.To == ActivityStatus.ApprovedStatus)
        {
            await unitOfWork.DbContext.InsertOutboxMessage(new ActivityApprovedIntegrationEvent(notification.Item.Id, notification.DateOccurred.DateTime));
        }
        await unitOfWork.DbContext.InsertOutboxMessage(new ActivityTransitionedIntegrationEvent(notification.Item.Id, notification.From.Name, notification.To.Name, notification.DateOccurred.DateTime));
    }
}