using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

/// <summary>
/// Dedicated consumer for local usage telemetry. Runs on its own queue so that the
/// (potentially chatty) telemetry stream is isolated from functional message processing.
/// </summary>
internal class TelemetryBackgroundService(IServiceProvider provider, IConfiguration configuration, IOptions<RabbitSettings> options) : BackgroundService
{
    private BuiltinHandlerActivator? _activator;
    private IBus? _bus;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _activator = new BuiltinHandlerActivator();

        _activator.Handle<RecordUsageConsumer>(provider);

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        _bus = Configure.With(_activator)
            .Logging(l => l.MicrosoftExtensionsLogging(loggerFactory))
            .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("rabbit"), options.Value.TelemetryService)
                .ExchangeNames(options.Value.DirectExchange, options.Value.TopicExchange))
            .Options(o =>
            {
                o.SetNumberOfWorkers(2);
                o.SetMaxParallelism(4);
                o.RetryStrategy(maxDeliveryAttempts: options.Value.Retries);
            })
            .Start();

        await _bus.Subscribe<UsageTrackedIntegrationEvent>();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();
        _activator?.Dispose();
        return Task.CompletedTask;
    }
}
