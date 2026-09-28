using System.Text.Json;
using Amazon.SQS.Model;
using Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents;
using Cfo.Cats.Infrastructure.Configurations;
using Microsoft.Extensions.Hosting;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Background service that long-polls the HMPPS SQS queue, unwraps the SNS envelope, deserializes the
/// HMPPS notification, dispatches it to the appropriate handler, and deletes the message only after it
/// has been processed successfully. Processing failures are logged and the message is left on the
/// queue (to become visible again after the visibility timeout) without stopping consumption.
/// </summary>
public class HmppsEventConsumer(
    IServiceProvider serviceProvider,
    ISqsClientProvider sqsClientProvider,
    IOptions<HmppsExternalApiOptions> options,
    ILogger<HmppsEventConsumer> logger) : BackgroundService, IHmppsEventConsumer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HmppsExternalEventsOptions _events = options.Value.Events;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("HMPPS event consumer starting for queue {QueueUrl}", _events.QueueUrl);

        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
                await PollOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown.
                break;
            }
            catch (Exception ex)
            {
                // Never let an unexpected failure stop consumption; back off briefly and retry.
                logger.LogError(ex, "Error polling the HMPPS SQS queue; retrying shortly");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        logger.LogInformation("HMPPS event consumer stopping");
    }

    private async Task PollOnceAsync(CancellationToken stoppingToken)
    {
        var client = await sqsClientProvider.GetClientAsync(stoppingToken);

        var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = _events.QueueUrl,
            MaxNumberOfMessages = _events.MaxNumberOfMessages,
            WaitTimeSeconds = _events.WaitTimeSeconds
        }, stoppingToken);

        if (response.Messages is null || response.Messages.Count == 0)
        {
            return;
        }

        foreach (var message in response.Messages)
        {
            await ProcessMessageAsync(client, message, stoppingToken);
        }
    }

    private async Task ProcessMessageAsync(Amazon.SQS.IAmazonSQS client, Message message, CancellationToken stoppingToken)
    {
        try
        {
            var notification = Deserialize(message.Body);

            if (notification is null)
            {
                logger.LogWarning("Could not deserialize HMPPS notification; leaving message {MessageId} on the queue", message.MessageId);
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IHmppsEventDispatcher>();
            await dispatcher.Dispatch(notification, stoppingToken);

            // Only delete after successful processing.
            await client.DeleteMessageAsync(_events.QueueUrl, message.ReceiptHandle, stoppingToken);
        }
        catch (Exception ex)
        {
            // Leave the message on the queue so it becomes visible again after the visibility timeout.
            logger.LogError(ex, "Failed to process HMPPS message {MessageId}; leaving on queue for retry", message.MessageId);
        }
    }

    private static HmppsNotification? Deserialize(string body)
    {
        // Messages arrive wrapped in an SNS envelope: { "Message": "{...actual event payload...}" }.
        var envelope = JsonSerializer.Deserialize<SnsEnvelope>(body, SerializerOptions);

        var payload = string.IsNullOrWhiteSpace(envelope?.Message) ? body : envelope!.Message!;

        return JsonSerializer.Deserialize<HmppsNotification>(payload, SerializerOptions);
    }
}
