using Cfo.Cats.Application.Features.Announcements.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Identity.MessageBus;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.ParticipantLabels.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Participants.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Participants.MessageBus;
using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.PRIs.IntegrationEventHandlers;
using Cfo.Cats.Infrastructure.Messaging.RabbitMq;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

/// <summary>
/// Declares which handlers each consuming service runs. Each service owns a queue; the queue is bound to the
/// events its handlers handle, so a service never receives events it has no handler for.
/// </summary>
internal static class MessageConsumers
{
    /// <summary>
    /// A special consumer that always runs in CATS. It allows the UI to react to integration events,
    /// and every running copy of CATS reacts (each has its own transient queue).
    /// </summary>
    public static IServiceCollection AddCatsConsumer(this IServiceCollection services, RabbitSettings settings)
        => services.AddRabbitMqConsumer(
            settings.CatsService,
            c => c.Handle<ContractTargetsChangedConsumer>(),
            transient: true);

    public static IServiceCollection AddOvernightConsumer(this IServiceCollection services, RabbitSettings settings, OvernightServiceSettings overnight)
        => services.AddRabbitMqConsumer(
            settings.OvernightService,
            c =>
            {
                // no one is using the server at night.
                c.MaxConcurrency = overnight.Parallelism;
                c.Handle<SyncParticipantCommandHandler>()
                    .Handle<NotifyInactiveUserCommandHandler>();
            });

    public static IServiceCollection AddTasksConsumer(this IServiceCollection services, RabbitSettings settings)
        => services.AddRabbitMqConsumer(
            settings.TasksService,
            c =>
            {
                c.MaxConcurrency = 4;
                c
                    .Handle<PriTaskCompletedWatcherConsumer>()
                    .Handle<RaisePaymentsAfterApprovalConsumer>()
                    .Handle<RecordEnrolmentReturnedFeedbackConsumer>()
                    .Handle<RecordEnrolmentAdvisoryFeedbackConsumer>()
                    .Handle<RecordActivityReturnedFeedbackConsumer>()
                    .Handle<RecordActivityAdvisoryFeedbackConsumer>()
                    .Handle<RecordArchivedCaseConsumer>()
                    .Handle<CloseOffLastArchivedCaseEntry>()
                    .Handle<RecordVeteranLabelStatusConsumer>()
                    .Handle<RecordGangMemberLabelStatusConsumer>()
                    .Handle<RecordCareLeaverLabelStatusConsumer>()
                    .Handle<RecordDisabledLabelStatusConsumer>()
                    .Handle<RecordHomelessLabelStatusConsumer>()
                    .Handle<RecordNeurodiverseLabelStatusConsumer>()
                    .Handle<ParticipantCreatedIntegrationEventConsumer>()
                    .Handle<NotifyUsersOnAnnouncementPublishedConsumer>();
            });

    public static IServiceCollection AddPaymentConsumer(this IServiceCollection services, RabbitSettings settings)
        => services.AddRabbitMqConsumer(
            settings.PaymentService,
            c =>
            {
                // Only one instance of the service processes messages at a time to prevent duplicate payments.
                c.SingleActiveConsumer = true;
                c
                    .Handle<RecordActivityPaymentConsumer>()
                    .Handle<RecordEducationPayment>()
                    .Handle<RecordEmploymentPayment>()
                    .Handle<RecordEnrolmentPaymentConsumer>()
                    .Handle<RecordHubInductionPaymentConsumer>()
                    .Handle<RecordPreReleaseSupportPayment>()
                    .Handle<RecordThroughTheGatePaymentConsumer>()
                    .Handle<RecordWingInductionPaymentConsumer>()
                    .Handle<RecordReassessmentPaymentConsumer>()
                    .Handle<RecordCpmScores>()
                    .Handle<GenerateOutcomeQualityDipSampleDocument>()
                    .Handle<RecordParticipantEngagementConsumer>();
            });

    public static IServiceCollection AddDocumentsConsumer(this IServiceCollection services, RabbitSettings settings)
        => services.AddRabbitMqConsumer(
            settings.DocumentService,
            c =>
            {
                c
                    .Handle<DocumentExportCaseWorkloadIntegrationEventConsumer>()
                    .Handle<DocumentExportKeyValuesIntegrationEventConsumer>()
                    .Handle<DocumentExportRiskDueAggregateIntegrationEventConsumer>()
                    .Handle<DocumentExportRiskDueIntegrationEventConsumer>()
                    .Handle<DocumentExportParticipantsIntegrationEventConsumer>()
                    .Handle<DocumentExportPqaActivitiesIntegrationEventConsumer>()
                    .Handle<DocumentExportActivitiesIntegrationEventConsumer>()
                    .Handle<DocumentExportPqaEnrolmentsIntegrationEventConsumer>()
                    .Handle<DocumentExportActivityPaymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportEducationPaymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportEmploymentPaymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportEnrolmentPaymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportInductionPaymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportSupportAndReferralPaymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportEnrolmentPaymentsSummaryIntegrationEventConsumer>()
                    .Handle<DocumentExportInductionPaymentsSummaryIntegrationEventConsumer>()
                    .Handle<DocumentExportSupportAndReferralPaymentsSummaryIntegrationEventConsumer>()
                    .Handle<DocumentExportActivityPaymentsSummaryIntegrationEventConsumer>()
                    .Handle<DocumentExportEducationPaymentsSummaryIntegrationEventConsumer>()
                    .Handle<DocumentExportEmploymentPaymentsSummaryIntegrationEventConsumer>()
                    .Handle<DocumentExportLatestParticipantEngagementsIntegrationEventConsumer>()
                    .Handle<DocumentExportCumulativesIntegrationEventConsumer>()
                    .Handle<DocumentExportOutcomeQualityDipSampleIntegrationEventConsumer>()
                    .Handle<DocumentExportProviderFeedbackIntegrationEventConsumer>()
                    .Handle<DocumentExportArchivedCasesBehaviourIntegrationEventConsumer>()
                    .Handle<DocumentExportInitiativeObjectivesDashboardIntegrationEventConsumer>()
                    .Handle<DocumentExportInitiativesIntegrationEventConsumer>()
                    .Handle<DocumentExportUsersIntegrationEventConsumer>()
                    .Handle<DocumentExportIdentityAuditTrailsIntegrationEventConsumer>()
                    .Handle<DocumentExportActivePrisIntegrationEventConsumer>()
                    .Handle<DocumentExportEngagementsByLocationIntegrationEventConsumer>()
                    .Handle<DocumentExportTasksDueIntegrationEventConsumer>()
                    .Handle<DocumentExportQaOfficerStatsIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceActivitiesIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceEducationAndTrainingIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceEmploymentsIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceEnrolmentsIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceInductionsIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceReassessmentsIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceRecentApprovedActivitiesIntegrationEventConsumer>()
                    .Handle<DocumentExportPerformanceSupportAndReferralIntegrationEventConsumer>();
            });

    public static IServiceCollection AddTelemetryConsumer(this IServiceCollection services, RabbitSettings settings)
        => services.AddRabbitMqConsumer(
            settings.TelemetryService,
            c =>
            {
                c.MaxConcurrency = 4;
                c.Handle<RecordUsageConsumer>();
            });
}
