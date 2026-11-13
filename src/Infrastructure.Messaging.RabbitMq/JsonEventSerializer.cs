using System.Text.Json;
using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Infrastructure.Messaging.RabbitMq;

/// <summary>
/// Uses the same default System.Text.Json settings as the outbox so stored content round-trips unchanged.
/// </summary>
public sealed class JsonEventSerializer : IEventSerializer
{
    public byte[] Serialize(IIntegrationEvent integrationEvent) =>
        JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType());

    public IIntegrationEvent Deserialize(ReadOnlySpan<byte> payload, Type eventType) =>
        (IIntegrationEvent?)JsonSerializer.Deserialize(payload, eventType)
        ?? throw new InvalidOperationException($"Unable to deserialize message payload as {eventType.FullName}.");
}
