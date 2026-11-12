namespace Cfo.Cats.Application.Messaging;

/// <summary>
/// Handles an integration event. Delivery is at-least-once, so implementations must be idempotent.
/// </summary>
public interface IIntegrationEventHandler<in TEvent> where TEvent : IIntegrationEvent
{
    Task Handle(TEvent integrationEvent, CancellationToken cancellationToken);
}
