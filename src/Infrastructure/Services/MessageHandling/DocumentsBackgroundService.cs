using Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

internal class DocumentsBackgroundService(IServiceProvider provider, IConfiguration configuration, IOptions<RabbitSettings> options) : BackgroundService
{

    private BuiltinHandlerActivator? _activator;
    private IBus? _bus;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _activator = new BuiltinHandlerActivator();

       _activator.Handle<DocumentExportCaseWorkloadIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportKeyValuesIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportRiskDueAggregateIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportRiskDueIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportParticipantsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportPqaActivitiesIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportPqaEnrolmentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportActivityPaymentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportEducationPaymentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportEmploymentPaymentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportEnrolmentPaymentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportInductionPaymentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportSupportAndReferralPaymentsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportLatestParticipantEngagementsIntegrationEventConsumer>(provider);
       _activator.Handle<DocumentExportCumulativesIntegrationEventConsumer>(provider);

        _bus = Configure.With(_activator)
            .Transport(t => t.UseRabbitMq(configuration.GetConnectionString("rabbit"), options.Value.DocumentService)
                .ExchangeNames(options.Value.DirectExchange, options.Value.TopicExchange))
            .Options(o =>
            {
                o.SetNumberOfWorkers(1);
                o.SetMaxParallelism(1);
                o.RetryStrategy(maxDeliveryAttempts: options.Value.Retries);
            })

            .Start();

        await _bus.Subscribe<ExportDocumentIntegrationEvent>();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _bus?.Dispose();
        _activator?.Dispose();
        return Task.CompletedTask;
    }
}