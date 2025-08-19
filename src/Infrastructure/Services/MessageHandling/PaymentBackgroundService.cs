using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Application.Features.QualityAssurance.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

public class PaymentBackgroundService(IServiceProvider provider, IConfiguration configuration, IOptions<RabbitSettings> options) : BackgroundService
{
    private BuiltinHandlerActivator? _activator;
    private IBus? _bus;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _activator = new BuiltinHandlerActivator();

        _activator.Handle<RecordActivityPaymentConsumer>(provider)
            .Handle<RecordEducationPayment>(provider)
            .Handle<RecordEmploymentPayment>(provider)
            .Handle<RecordEnrolmentPaymentConsumer>(provider)
            .Handle<RecordHubInductionPaymentConsumer>(provider)
            .Handle<RecordPreReleaseSupportPayment>(provider)
            .Handle<RecordThroughTheGatePaymentConsumer>(provider)
            .Handle<RecordWingInductionPaymentConsumer>(provider)
            .Handle<RecordReassessmentPaymentConsumer>(provider)
            .Handle<RecordCpmScores>(provider);

        _bus = Configure.With(_activator)
            .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("rabbit"), options.Value.PaymentService)
                .ExchangeNames(options.Value.DirectExchange, options.Value.TopicExchange))
            .Options(o =>
            {
                o.SetNumberOfWorkers(1);
                o.SetMaxParallelism(1);
                o.RetryStrategy(maxDeliveryAttempts: options.Value.Retries);
            })
            .Start();

        await _bus.Subscribe<ActivityApprovedIntegrationEvent>();
        await _bus.Subscribe<EnrolmentApprovedAtQaIntegrationEvent>();
        await _bus.Subscribe<HubInductionCreatedIntegrationEvent>();
        await _bus.Subscribe<PRIAssignedIntegrationEvent>();
        await _bus.Subscribe<PRIThroughTheGateCompletedIntegrationEvent>();
        await _bus.Subscribe<WingInductionCreatedIntegrationEvent>();
        await _bus.Subscribe<AssessmentScoredIntegrationEvent>();
        await _bus.Subscribe<OutcomeQualityDipSampleVerifyingIntegrationEvent>();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();
        _activator?.Dispose();
        return Task.CompletedTask;
    }
}