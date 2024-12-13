using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Inductions.EventHandlers;

public class PublishWingInductionCreatedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<WingInductionCreatedDomainEvent>
{
    public async Task Handle(WingInductionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var e = new WingInductionCreatedIntegrationEvent(notification.Item.Id);
        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}

