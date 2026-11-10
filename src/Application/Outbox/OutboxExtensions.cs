namespace Cfo.Cats.Application.Outbox;

public static class OutboxExtensions
{
    public static async Task InsertOutboxMessage<T>(
        this IApplicationDbContext context,
        T message,
        Guid? outboxMessageId = null,
        DateTime? occurredOnUtc = null)
        where T : notnull, IntegrationEvent
    {
        var outboxMessage = new OutboxMessage
        {
            Id = outboxMessageId ?? Guid.CreateVersion7(),
            Type = message.GetType().FullName!,
            Content = JsonSerializer.Serialize(message),
            OccurredOnUtc = occurredOnUtc ?? DateTime.UtcNow
        };

        await context.OutboxMessages.AddAsync(outboxMessage!);
    }
}