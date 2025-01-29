using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PRIs.EventHandlers;

public class RaisePRIAssignedIntegrationEvent(IUnitOfWork unitOfWork) : INotificationHandler<PRIAssignedDomainEvent>
{
    public async Task Handle(PRIAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new PRIAssignedIntegrationEvent(notification.Item.Id, notification.DateOccurred.DateTime);
        await unitOfWork.DbContext.InsertOutboxMessage(@event);
    }
}