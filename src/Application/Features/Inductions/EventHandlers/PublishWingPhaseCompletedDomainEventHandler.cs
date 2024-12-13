using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Inductions.EventHandlers;

public class PublishWingPhaseCompletedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<InductionPhaseCompletedDomainEvent>
{
    public async Task Handle(InductionPhaseCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var e = new WingPhaseCompletedIntegrationEvent(notification.InductionId, notification.Item.Number);
        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}