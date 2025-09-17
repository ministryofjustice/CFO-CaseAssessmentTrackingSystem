using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class PublishActivityApprovedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.From == ActivityStatus.ApprovedStatus)
        {
            await unitOfWork.DbContext.InsertOutboxMessage(new ActivityApprovedIntegrationEvent(notification.Item.Id, notification.DateOccurred.DateTime));
        }
    }
}