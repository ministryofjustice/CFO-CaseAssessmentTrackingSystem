using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Domain.Events;
using MassTransit;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class PublishEnrolmentApproval(IBus bus) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        var e = new ParticipantTransitionedIntegrationEvent(
            notification.Item.Id,
            notification.From.ToString(),
            notification.To.ToString()
        );
        await bus.Publish(e, cancellationToken);
    }
}