﻿using System.Text.Json;
using Cfo.Cats.Application.Outbox;
using Quartz;
using Rebus.Bus;


namespace Cfo.Cats.Infrastructure.Jobs;

public class PublishOutboxMessagesJob(IUnitOfWork unitOfWork, ILogger<PublishOutboxMessagesJob> logger, IBus bus)
    : IJob
{
    private const int BatchSize = 10;

    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);


    public static readonly JobKey Key = new JobKey(name: nameof(PublishOutboxMessagesJob));
    public static readonly string Description = "A job to publish outbox messages to the queue";


    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount > 3)
        {
            logger.LogWarning($"Quartz Job - {Key}: failed to complete within 3 tries, aborting...");
            return;
        }

        if (!await Semaphore.WaitAsync(TimeSpan.Zero))
        {
            // Job is already running, skip this execution
            return;
        }

        try
        {
            var outboxMessages = unitOfWork.DbContext.OutboxMessages
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(BatchSize)
                .ToList();

            foreach (var outboxMessage in outboxMessages)
            {
                try
                {
                    var messageType = typeof(OutboxMessage).Assembly.GetType(outboxMessage.Type)!;
                    var deserializedMessage = JsonSerializer.Deserialize(outboxMessage.Content, messageType);

                    if (deserializedMessage is null)
                    {
                        throw new ApplicationException("Unable to deserialize message content");
                    }

                    //await bus.Advanced.Topics.Publish(outboxMessage.Type, deserializedMessage);

                    await bus.Publish(deserializedMessage);
                    
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                    outboxMessage.Error = ex.ToString();
                }
            }

            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"Quartz Job - {Key}: An unexpected error occurred executing job",
                refireImmediately: true, cause: ex);
        }
        finally
        {
            Semaphore.Release();
        }

    }
}