using Cfo.Cats.Application.Features.PathwayPlans.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PathwayPlans.EventHandlers
{
    internal class PublishTaskCompletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ObjectiveTaskCompletedDomainEvent>
    {
        public async Task Handle(ObjectiveTaskCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            await unitOfWork.DbContext.InsertOutboxMessage(new ObjectiveTaskCompletedIntegrationEvent(
                notification.Item.Id,
                notification.Item.ObjectiveId,
                notification.Item.IsMandatory,
                notification.Item.Index,
                notification.Item.CompletedStatus!.Name
            ));
        }
    }
}
