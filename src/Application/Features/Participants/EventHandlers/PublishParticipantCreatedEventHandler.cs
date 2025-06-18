using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

internal class PublishParticipantCreatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantCreatedDomainEvent>
{
    public async Task Handle(ParticipantCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.InsertOutboxMessage(new ParticipantCreatedIntegrationEvent(notification.Item.Id, notification.Item.Created ?? DateTime.UtcNow));
    }
}