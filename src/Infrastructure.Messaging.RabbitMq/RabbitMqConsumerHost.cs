using Cfo.Cats.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Hosts one consumer: declares its topology, binds its queue to the events its handlers care about and
/// dispatches each message to every handler (in a fresh DI scope). Failures are retried with exponential
/// backoff via per-delay retry queues and finally routed to a dead-letter queue.
/// </summary>
/// <remarks>
/// Topology for queue <c>q</c> with N allowed deliveries: <c>q</c>, <c>q.retry.1</c> .. <c>q.retry.(N-1)</c>
/// (TTL queues that dead-letter back into <c>q</c>) and <c>q.dlq</c>. Retried and dead-lettered copies are sent via
/// the default exchange straight to those queues, so other consumers never see them.
/// </remarks>
public sealed class RabbitMqConsumerHost(
    ConsumerRegistration registration,
    RabbitMqConnection connection,
    IServiceScopeFactory scopeFactory,
    IEventSerializer serializer,
    IEventNameResolver nameResolver,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConsumerHost> logger) : BackgroundService
{
    private delegate Task HandlerInvoker(object handler, IIntegrationEvent integrationEvent, CancellationToken cancellationToken);

    private readonly string _queue = registration.Transient
        ? $"{registration.QueueName}-{Guid.NewGuid():N}"
        : registration.QueueName;

    private readonly Dictionary<Type, List<(Type HandlerType, HandlerInvoker Invoke)>> _handlers = BuildHandlerMap(registration.HandlerTypes);

    private IChannel? _channel;

    private int MaxDeliveries => registration.Transient ? 1 : Math.Max(1, options.Value.Retries);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = TimeSpan.FromSeconds(2);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await StartConsumingAsync(stoppingToken);
                return;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Unable to start RabbitMQ consumer {Queue}; retrying in {Delay}", _queue, delay);
                await Task.Delay(delay, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 30));
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }
    }

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var conn = await connection.GetConnectionAsync(cancellationToken);
        var channel = await conn.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: false,
                publisherConfirmationTrackingEnabled: false,
                consumerDispatchConcurrency: (ushort)registration.MaxConcurrency),
            cancellationToken);

        await DeclareTopologyAsync(channel, cancellationToken);
        await channel.BasicQosAsync(0, (ushort)registration.MaxConcurrency, global: false, cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, args) => OnReceivedAsync(channel, args, cancellationToken);

        await channel.BasicConsumeAsync(_queue, autoAck: false, consumer, cancellationToken);
        _channel = channel;

        logger.LogInformation("Consuming queue {Queue} with {HandlerCount} handler(s) for {EventCount} event type(s)",
            _queue, registration.HandlerTypes.Count, _handlers.Count);
    }

    private async Task DeclareTopologyAsync(IChannel channel, CancellationToken cancellationToken)
    {
        var exchange = options.Value.Exchange;
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            _queue,
            durable: !registration.Transient,
            exclusive: false,
            autoDelete: registration.Transient,
            arguments: registration.SingleActiveConsumer
                ? new Dictionary<string, object?> { ["x-single-active-consumer"] = true }
                : null,
            cancellationToken: cancellationToken);

        foreach (var eventType in _handlers.Keys)
        {
            await channel.QueueBindAsync(_queue, exchange, nameResolver.GetName(eventType), cancellationToken: cancellationToken);
        }

        if (registration.Transient)
        {
            return;
        }

        await channel.QueueDeclareAsync(DeadLetterQueue, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);

        for (var attempt = 1; attempt < MaxDeliveries; attempt++)
        {
            await channel.QueueDeclareAsync(
                RetryQueue(attempt),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    ["x-message-ttl"] = (int)RetryDelay(attempt).TotalMilliseconds,
                    ["x-dead-letter-exchange"] = string.Empty,
                    ["x-dead-letter-routing-key"] = _queue
                },
                cancellationToken: cancellationToken);
        }
    }

    private async Task OnReceivedAsync(IChannel channel, BasicDeliverEventArgs args, CancellationToken cancellationToken)
    {
        var messageId = args.BasicProperties.MessageId;
        var correlationId = args.BasicProperties.CorrelationId;
        var attempt = GetAttempt(args.BasicProperties);

        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["MessageId"] = messageId,
            ["CorrelationId"] = correlationId,
            ["Queue"] = _queue
        });

        try
        {
            logger.LogInformation("Received integration event {EventType} (attempt {Attempt})", args.BasicProperties.Type, attempt);

            await HandleAsync(args, cancellationToken);
            await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Shutting down: leave the message unacknowledged so it is redelivered.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Integration event {EventType} failed on attempt {Attempt} of {MaxAttempts}",
                args.BasicProperties.Type, attempt, MaxDeliveries);

            try
            {
                await RetryOrDeadLetterAsync(channel, args, attempt, ex, cancellationToken);
                await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken);
            }
            catch (Exception routingException) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogCritical(routingException, "Unable to retry or dead-letter integration event {EventType}; returning it to the queue",
                    args.BasicProperties.Type);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
                await channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: true, cancellationToken);
            }
        }
    }

    private async Task HandleAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
    {
        var eventName = args.BasicProperties.Type
            ?? throw new InvalidOperationException("Message has no event type.");
        var version = GetVersion(args.BasicProperties);

        var eventType = nameResolver.ResolveType(eventName, version);
        var integrationEvent = serializer.Deserialize(args.Body.Span, eventType);

        if (!_handlers.TryGetValue(eventType, out var handlers))
        {
            throw new InvalidOperationException($"No handlers in queue {_queue} for {eventName}.");
        }

        await using var scope = scopeFactory.CreateAsyncScope();

        foreach (var (handlerType, invoke) in handlers)
        {
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
            await invoke(handler, integrationEvent, cancellationToken);
        }
    }

    private async Task RetryOrDeadLetterAsync(IChannel channel, BasicDeliverEventArgs args, int attempt, Exception exception, CancellationToken cancellationToken)
    {
        if (registration.Transient)
        {
            logger.LogWarning("Dropping failed integration event {EventType} on transient queue", args.BasicProperties.Type);
            return;
        }

        var properties = new BasicProperties(args.BasicProperties)
        {
            Headers = new Dictionary<string, object?>(args.BasicProperties.Headers ?? new Dictionary<string, object?>())
        };

        if (attempt < MaxDeliveries)
        {
            properties.Headers[MessageHeaders.Attempt] = attempt + 1;
            await channel.BasicPublishAsync(string.Empty, RetryQueue(attempt), mandatory: true, properties, args.Body, cancellationToken);

            logger.LogWarning("Scheduled retry {NextAttempt} of {MaxAttempts} for {EventType} in {Delay}",
                attempt + 1, MaxDeliveries, args.BasicProperties.Type, RetryDelay(attempt));
            return;
        }

        properties.Headers[MessageHeaders.Error] = Truncate(exception.ToString(), 4000);
        await channel.BasicPublishAsync(string.Empty, DeadLetterQueue, mandatory: true, properties, args.Body, cancellationToken);

        logger.LogError("Dead-lettered integration event {EventType} to {DeadLetterQueue} after {Attempts} attempt(s)",
            args.BasicProperties.Type, DeadLetterQueue, attempt);
    }

    private string DeadLetterQueue => $"{_queue}.dlq";

    private string RetryQueue(int attempt) => $"{_queue}.retry.{attempt}";

    private TimeSpan RetryDelay(int attempt) =>
        TimeSpan.FromSeconds(options.Value.RetryBaseDelaySeconds * Math.Pow(2, attempt - 1));

    private static int GetAttempt(IReadOnlyBasicProperties properties) =>
        TryGetInt(properties, MessageHeaders.Attempt) ?? 1;

    private static int GetVersion(IReadOnlyBasicProperties properties) =>
        TryGetInt(properties, MessageHeaders.Version) ?? 1;

    private static int? TryGetInt(IReadOnlyBasicProperties properties, string header) =>
        properties.Headers?.TryGetValue(header, out var value) == true && value is not null
            ? Convert.ToInt32(value)
            : null;

    private static string Truncate(string value, int length) =>
        value.Length <= length ? value : value[..length];

    private static Dictionary<Type, List<(Type, HandlerInvoker)>> BuildHandlerMap(IEnumerable<Type> handlerTypes)
    {
        var map = new Dictionary<Type, List<(Type, HandlerInvoker)>>();

        foreach (var handlerType in handlerTypes)
        {
            foreach (var handlerInterface in handlerType.GetInterfaces().Where(ConsumerBuilder.IsHandlerInterface))
            {
                var eventType = handlerInterface.GetGenericArguments()[0];

                var invoker = (HandlerInvoker)typeof(RabbitMqConsumerHost)
                    .GetMethod(nameof(CreateInvoker), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(eventType)
                    .Invoke(null, null)!;

                if (!map.TryGetValue(eventType, out var list))
                {
                    map[eventType] = list = [];
                }

                list.Add((handlerType, invoker));
            }
        }

        return map;
    }

    private static HandlerInvoker CreateInvoker<TEvent>() where TEvent : IIntegrationEvent =>
        (handler, integrationEvent, cancellationToken) =>
            ((IIntegrationEventHandler<TEvent>)handler).Handle((TEvent)integrationEvent, cancellationToken);
}
