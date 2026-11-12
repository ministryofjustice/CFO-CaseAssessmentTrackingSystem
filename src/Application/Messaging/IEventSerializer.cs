namespace Cfo.Cats.Application.Messaging;

public interface IEventSerializer
{
    byte[] Serialize(IIntegrationEvent integrationEvent);
    IIntegrationEvent Deserialize(ReadOnlySpan<byte> payload, Type eventType);
}
