namespace Cfo.Cats.Application.Outbox;

internal static class OutboxExtensions
{
    internal static async Task InsertOutboxMessage<T>(
        this IApplicationDbContext context,
        T message)
        where T : notnull
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = message.GetType().FullName!,
            Content = JsonSerializer.Serialize(message),
            OccurredOnUtc = DateTime.UtcNow
        };

        await context.OutboxMessages.AddAsync(outboxMessage!);
    }
}