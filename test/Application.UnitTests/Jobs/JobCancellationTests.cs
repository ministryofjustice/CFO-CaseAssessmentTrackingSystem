using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Infrastructure.Configurations;
using Cfo.Cats.Infrastructure.Jobs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Quartz;
using Rebus.Bus;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Jobs;

public class JobCancellationTests
{
    [TestCase(typeof(ArchiveParticipantsJob))]
    [TestCase(typeof(DisableDormantAccountsJob))]
    [TestCase(typeof(GenerateOutcomeQualityDipSamplesJob))]
    [TestCase(typeof(MonitorSuspiciousLoginActivityJob))]
    [TestCase(typeof(NotifyAccountDeactivationJob))]
    [TestCase(typeof(PublishOutboxMessagesJob))]
    [TestCase(typeof(SyncParticipantsJob))]
    public async Task Execute_Should_Propagate_Cancellation_Without_Requesting_A_Retry(Type jobType)
    {
        using var cancellation = new CancellationTokenSource();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(work => work.DbContext).Returns(() =>
        {
            cancellation.Cancel();
            throw new OperationCanceledException(cancellation.Token);
        });

        IJob job = jobType.Name switch
        {
            nameof(ArchiveParticipantsJob) => new ArchiveParticipantsJob(
                NullLogger<ArchiveParticipantsJob>.Instance, unitOfWork.Object, Mock.Of<IDomainEventDispatcher>()),
            nameof(DisableDormantAccountsJob) => new DisableDormantAccountsJob(
                NullLogger<DisableDormantAccountsJob>.Instance, unitOfWork.Object),
            nameof(GenerateOutcomeQualityDipSamplesJob) => new GenerateOutcomeQualityDipSamplesJob(
                NullLogger<GenerateOutcomeQualityDipSamplesJob>.Instance, unitOfWork.Object,
                Options.Create(new OutcomeQualityDipSampleSettings())),
            nameof(MonitorSuspiciousLoginActivityJob) => new MonitorSuspiciousLoginActivityJob(
                NullLogger<MonitorSuspiciousLoginActivityJob>.Instance, unitOfWork.Object,
                Mock.Of<ICommunicationsService>(), Options.Create(new SuspiciousLoginMonitoringOptions())),
            nameof(NotifyAccountDeactivationJob) => new NotifyAccountDeactivationJob(
                NullLogger<NotifyAccountDeactivationJob>.Instance, unitOfWork.Object, Mock.Of<IBus>()),
            nameof(PublishOutboxMessagesJob) => new PublishOutboxMessagesJob(
                unitOfWork.Object, NullLogger<PublishOutboxMessagesJob>.Instance, Mock.Of<IBus>()),
            nameof(SyncParticipantsJob) => new SyncParticipantsJob(
                NullLogger<SyncParticipantsJob>.Instance, unitOfWork.Object, Mock.Of<IBus>()),
            _ => throw new ArgumentOutOfRangeException(nameof(jobType))
        };

        var exception = await Should.ThrowAsync<OperationCanceledException>(
            () => job.Execute(Mock.Of<IJobExecutionContext>(), cancellation.Token).AsTask());

        exception.CancellationToken.ShouldBe(cancellation.Token);
    }
}