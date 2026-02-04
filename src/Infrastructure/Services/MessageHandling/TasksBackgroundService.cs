using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Features.PathwayPlans.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.IntegrationEventHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

internal class TasksBackgroundService(IServiceProvider provider, IConfiguration configuration, IOptions<RabbitSettings> options) : BackgroundService
{
    private BuiltinHandlerActivator? _activator;
    private IBus? _bus;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _activator = new BuiltinHandlerActivator();

        _activator.Handle<PriTaskCompletedWatcherConsumer>(provider)
            .Handle<RaisePaymentsAfterApprovalConsumer>(provider)
            .Handle<RecordEnrolmentReturnedFeedbackConsumer>(provider)
            .Handle<RecordEnrolmentAdvisoryFeedbackConsumer>(provider)
            .Handle<RecordActivityReturnedFeedbackConsumer>(provider)
            .Handle<RecordActivityAdvisoryFeedbackConsumer>(provider);

        _bus = Configure.With(_activator)
            .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("rabbit"), options.Value.TasksService)
                .ExchangeNames(options.Value.DirectExchange, options.Value.TopicExchange))
            .Options(o =>
            {
                o.SetNumberOfWorkers(2);
                o.SetMaxParallelism(4);
                o.RetryStrategy(maxDeliveryAttempts: options.Value.Retries);
            })
            .Start();

        await _bus.Subscribe<ObjectiveTaskCompletedIntegrationEvent>();
        await _bus.Subscribe<ParticipantTransitionedIntegrationEvent>();
        await _bus.Subscribe<ActivityTransitionedIntegrationEvent>();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();
        _activator?.Dispose();
        return Task.CompletedTask;
    }
}
