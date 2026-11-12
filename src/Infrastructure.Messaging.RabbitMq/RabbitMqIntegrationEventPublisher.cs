using System.Diagnostics;
using Cfo.Cats.Application.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqIntegrationEventPublisher(
    RabbitMqConnection connection,
    IEventSerializer serializer,
    IEventNameResolver nameResolver,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqIntegrationEventPublisher> logger) : IIntegrationEventPublisher, IAsyncDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IChannel? _channel;

    public async Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var eventType = integrationEvent.GetType();
        var name = nameResolver.GetName(eventType);
        var version = nameResolver.GetVersion(eventType);
        var correlationId = Activity.Current?.TraceId.ToString();

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = integrationEvent.MessageId.ToString(),
            CorrelationId = correlationId,
            Type = name,
            Timestamp = new AmqpTimestamp(new DateTimeOffset(DateTime.SpecifyKind(integrationEvent.OccurredOn, DateTimeKind.Utc)).ToUnixTimeSeconds()),
            Headers = new Dictionary<string, object?>
            {
                [MessageHeaders.Version] = version,
                [MessageHeaders.OccurredOnUtc] = integrationEvent.OccurredOn.ToString("O")
            }
        };

        var body = serializer.Serialize(integrationEvent);

        // Channels are not thread-safe; serialise publishes. Throughput needs here are modest.
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var channel = await GetChannelAsync(cancellationToken);
            await channel.BasicPublishAsync(options.Value.Exchange, name, mandatory: false, properties, body, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        logger.LogInformation("Published integration event {EventType} {MessageId} (correlation {CorrelationId})",
            name, integrationEvent.MessageId, correlationId);
    }

    internal async Task EnsureReadyAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await GetChannelAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        var conn = await connection.GetConnectionAsync(cancellationToken);
        _channel = await conn.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
            cancellationToken);

        await _channel.ExchangeDeclareAsync(options.Value.Exchange, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        _gate.Dispose();
    }
}
