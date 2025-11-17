using Cfo.Cats.EventBus.Abstractions;
using Cfo.Cats.PaymentProcessor.IntegrationEvents.Events;

namespace Cfo.Cats.PaymentProcessor.IntegrationEvents.EventHandling;

public class PaymentProcessedIntegrationEventHandler(ILogger<PaymentProcessedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<PaymentProcessedIntegrationEvent>
{
    public Task Handle(PaymentProcessedIntegrationEvent @event)
    {
        logger.LogDebug($"Handling integration event: {@event}");
        return Task.CompletedTask;
    }
}