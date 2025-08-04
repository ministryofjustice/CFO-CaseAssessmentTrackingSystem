namespace Cfo.Cats.Application.Outbox;

public static class OutboxExtensions
{
    public static async Task InsertOutboxMessage<T>(
        this IApplicationDbContext context,
        T message)
        where T : notnull
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.CreateVersion7(),
            Type = message.GetType().FullName!,
            Content = JsonSerializer.Serialize(message),
            OccurredOnUtc = DateTime.UtcNow
        };

        await context.OutboxMessages.AddAsync(outboxMessage!);
    }
}