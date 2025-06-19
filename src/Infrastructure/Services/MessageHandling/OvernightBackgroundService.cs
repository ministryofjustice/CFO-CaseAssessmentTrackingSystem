using Cfo.Cats.Application.Features.Participants.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

internal class OvernightBackgroundService(IServiceProvider provider, IConfiguration configuration, IOptions<RabbitSettings> options) : BackgroundService
{
    private BuiltinHandlerActivator? _activator;
    private IBus? _bus;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _activator = new BuiltinHandlerActivator();
        _activator.Handle<SyncParticipantCommandHandler>(provider);

        _bus = Configure.With(_activator)
            .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("rabbit"), options.Value.OvernightService)
                .ExchangeNames(options.Value.DirectExchange, options.Value.TopicExchange))
            .Options(o =>
            {
                // no one is using the server at night.
                o.SetNumberOfWorkers(5);
                o.SetMaxParallelism(64);
                o.RetryStrategy(maxDeliveryAttempts: options.Value.Retries);
            })
            .Start();

        await _bus.Subscribe<SyncParticipantCommand>();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();
        _activator?.Dispose();
        return Task.CompletedTask;
    }
}