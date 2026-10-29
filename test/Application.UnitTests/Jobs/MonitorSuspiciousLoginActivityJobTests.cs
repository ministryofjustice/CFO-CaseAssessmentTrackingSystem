using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Domain.Entities;
using Cfo.Cats.Infrastructure.Jobs;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Quartz;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Jobs;

public class MonitorSuspiciousLoginActivityJobTests
{
    [TestCase(true)]
    [TestCase(false)]
    public async Task Execute_WhenCancelledAfterSavingAlertsOrDuringSending_ShouldSendEmailToAllRecipients(
        bool cancelAfterSaving)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new ApplicationDbContext(options);
        dbContext.IdentityAuditTrails.Add(IdentityAuditTrail.Create(
            "admin@example.com", null, IdentityActionType.UnknownUser, "192.0.2.10"));
        await dbContext.SaveChangesAsync();

        using var cancellation = new CancellationTokenSource();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(work => work.DbContext).Returns(dbContext);
        unitOfWork
            .Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(async token =>
            {
                var count = await dbContext.SaveChangesAsync(token);
                if (cancelAfterSaving)
                {
                    cancellation.Cancel();
                }
                return count;
            });

        var communicationsService = new Mock<ICommunicationsService>();
        communicationsService
            .Setup(service => service.SendLoginThresholdAlertEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => cancellation.Cancel())
            .Returns(Task.CompletedTask);

        var settings = new SuspiciousLoginMonitoringOptions
        {
            FailedAttemptThreshold = 1,
            EvaluationWindowSeconds = 300,
            DistributionList = ["first@example.com", "second@example.com"]
        };
        var job = new MonitorSuspiciousLoginActivityJob(
            NullLogger<MonitorSuspiciousLoginActivityJob>.Instance,
            unitOfWork.Object,
            communicationsService.Object,
            Options.Create(settings));

        await job.Execute(Mock.Of<IJobExecutionContext>(), cancellation.Token);

        foreach (var recipient in settings.DistributionList)
        {
            communicationsService.Verify(service => service.SendLoginThresholdAlertEmailAsync(
                recipient, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        dbContext.IdentityAuditTrails.Count(audit => audit.ActionType == IdentityActionType.SuspiciousActivityDetected)
            .ShouldBe(2);
    }
}