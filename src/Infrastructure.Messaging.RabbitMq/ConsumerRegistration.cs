using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Describes one consuming service: the queue it owns and the handlers that run for each message.
/// The set of events the queue is bound to is derived from the handlers' <see cref="IIntegrationEventHandler{TEvent}"/> interfaces.
/// </summary>
/// <param name="Transient">
/// A transient consumer gets its own auto-delete, non-durable queue (unique per process) so that every running
/// instance receives every event, and failures are logged and dropped instead of retried.
/// </param>
/// <param name="MaxConcurrency">Number of messages processed in parallel by this consumer.</param>
/// <param name="SingleActiveConsumer">
/// When true only one instance of the service consumes the queue at a time (others stand by), which is
/// used to prevent duplicate processing such as double payments.
/// </param>
public sealed record ConsumerRegistration(
    string QueueName,
    IReadOnlyList<Type> HandlerTypes,
    bool Transient,
    int MaxConcurrency = 1,
    bool SingleActiveConsumer = false);

public sealed class ConsumerBuilder
{
    private readonly List<Type> _handlerTypes = [];

    public ConsumerBuilder Handle<THandler>() where THandler : class
    {
        var handlerType = typeof(THandler);

        if (!handlerType.GetInterfaces().Any(IsHandlerInterface))
        {
            throw new InvalidOperationException($"{handlerType.Name} does not implement IIntegrationEventHandler<>.");
        }

        _handlerTypes.Add(handlerType);
        return this;
    }

    public int MaxConcurrency { get; set; } = 1;

    public bool SingleActiveConsumer { get; set; }

    internal IReadOnlyList<Type> HandlerTypes => _handlerTypes;

    internal static bool IsHandlerInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>);
}
