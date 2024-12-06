using System.Text.Json;
using Cfo.Cats.Application.Outbox;
using MassTransit;
using MassTransit.Logging;

namespace Cfo.Cats.Infrastructure.Services.Outbox;

internal sealed class OutboxProcessor(IApplicationDbContext dataSource, IPublishEndpoint publishEndpoint)
{
    private const int BatchSize = 10;

    public async Task<int> Execute(CancellationToken cancellationToken = default)
    {
        var outboxMessages = dataSource.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(BatchSize)
            .ToList();

        foreach (var outboxMessage in outboxMessages)
        {
            try
            {
                var messageType = typeof(Domain.Common.DomainEvent).Assembly.GetType(outboxMessage.Type)!;
                var deserializedMessage = JsonSerializer.Deserialize(outboxMessage.Content, messageType)!;

                await publishEndpoint.Publish(deserializedMessage, messageType, cancellationToken);

                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                outboxMessage.Error = ex.ToString();
            }
        }

        return outboxMessages.Count;
    }
}