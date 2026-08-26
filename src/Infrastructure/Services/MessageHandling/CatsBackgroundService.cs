using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

public class CatsBackgroundService(IServiceProvider provider, IConfiguration configuration, IOptions<RabbitSettings> options) : BackgroundService
{
     private BuiltinHandlerActivator? _activator;
    private IBus? _bus;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _activator = new BuiltinHandlerActivator();

        _activator.Handle<ContractTargetsChangedConsumer>(provider);

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        Guid key = Guid.NewGuid();

        _bus = Configure.With(_activator)
            .Logging(l => l.MicrosoftExtensionsLogging(loggerFactory))
            .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("rabbit"), $"{options.Value.CatsService}-{key}")
                .ExchangeNames(options.Value.DirectExchange, options.Value.TopicExchange)
                .InputQueueOptions(o =>
                {
                   o.SetAutoDelete(true); 
                }))
            .Options(o =>
            {
                o.SetNumberOfWorkers(1);
                o.SetMaxParallelism(1);
                o.RetryStrategy(maxDeliveryAttempts: options.Value.Retries);
            })

            .Start();

        await _bus.Subscribe<TargetsChangedIntegrationEvent>();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();
        _activator?.Dispose();

        await base.StopAsync(cancellationToken);
    }
}