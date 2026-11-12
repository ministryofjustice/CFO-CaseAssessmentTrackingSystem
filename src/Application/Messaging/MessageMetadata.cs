namespace Cfo.Cats.Application.Messaging;

public sealed record MessageMetadata(
    Guid MessageId,
    string EventType,
    int Version,
    DateTime OccurredOnUtc,
    string? CorrelationId = null,
    string? CausationId = null);
