using Cfo.Cats.EventBus.Abstractions;
using Cfo.Cats.PaymentProcessor.IntegrationEvents.Events;

namespace Cfo.Cats.PaymentProcessor.IntegrationEvents.EventHandling;

public class ParticipantTransitionedIntegrationEventHandler(ILogger<ParticipantTransitionedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<ParticipantTransitionedIntegrationEvent>
{
    public Task Handle(ParticipantTransitionedIntegrationEvent @event)
    {
        logger.LogInformation($"Handling integration event: {@event}");
        return Task.CompletedTask;
    }
}