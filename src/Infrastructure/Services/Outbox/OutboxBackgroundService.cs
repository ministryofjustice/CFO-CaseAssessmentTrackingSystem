using Microsoft.Extensions.Hosting;

namespace Cfo.Cats.Infrastructure.Services.Outbox;

internal class OutboxBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxBackgroundService> logger) : BackgroundService
{
    private const int OutboxProcessorFrequency = 7;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting OutboxBackgroundService...");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

                await outboxProcessor.Execute(stoppingToken);

                // Simulate running Outbox processing every N seconds
                await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequency), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("OutboxBackgroundService cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred in OutboxBackgroundService");
        }
        finally
        {
            logger.LogInformation("OutboxBackgroundService finished...");
        }
    }
}