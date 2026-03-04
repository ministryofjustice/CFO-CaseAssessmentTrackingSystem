using Cfo.Cats.Application.Features.Bios.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Bios.EventHandlers;

public class PublishBioSubmittedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<BioSubmittedDomainEvent>
{
    public async Task Handle(BioSubmittedDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.InsertOutboxMessage(new BioSubmittedIntegrationEvent( notification.Item.Id ));
    }
}