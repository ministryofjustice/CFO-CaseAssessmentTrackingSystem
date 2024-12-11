using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Common.IntegrationEvents;

public abstract record IntegrationEvent
{
    [JsonInclude] public Guid Id { get; private init; } = Guid.CreateVersion7();
    [JsonInclude] public DateTime CreationDate { get; private init; } = DateTime.UtcNow;
}