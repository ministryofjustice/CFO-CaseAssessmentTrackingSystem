using System.Data.Common;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Jobs;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Quartz;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Jobs;

public class NotifyAccountDeactivationJobTests
{
    private sealed class TestClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 11, 2, 12, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private sealed class CancelConnection(CancellationTokenSource cancellation) : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
            ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            cancellation.Cancel();
            throw new OperationCanceledException(cancellationToken);
        }
    }

    [Test]
    public async Task Execute_ShouldTranslateEligibilityQueryForSqlServerWithoutOpeningConnection()
    {
        using var cancellation = new CancellationTokenSource();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost;Database=WarningQueryTranslation;Integrated Security=true")
            .AddInterceptors(new CancelConnection(cancellation)).Options;
        await using var dbContext = new ApplicationDbContext(options);

        await Should.ThrowAsync<OperationCanceledException>(() => CreateJob(dbContext, new TestClock())
            .Execute(Mock.Of<IJobExecutionContext>(), cancellation.Token).AsTask());

        cancellation.IsCancellationRequested.ShouldBeTrue();
    }

    [TestCase(22, false)]
    [TestCase(23, true)]
    [TestCase(24, true)]
    [TestCase(31, true)]
    public async Task Execute_ShouldCatchUpUsingCalendarDays(int days, bool eligible)
    {
        await using var dbContext = CreateContext();
        var clock = new TestClock();
        dbContext.Users.Add(CreateUser("user", clock.Now.Date.AddDays(-days).AddHours(23)));
        await dbContext.SaveChangesAsync();

        await CreateJob(dbContext, clock).Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);

        dbContext.OutboxMessages.Count().ShouldBe(eligible ? 1 : 0);
    }

    [Test]
    public async Task Execute_ShouldRespectStatusAndActivityFallback()
    {
        await using var dbContext = CreateContext();
        var clock = new TestClock();
        var oldDate = clock.Now.Date.AddDays(-24);
        var inactive = CreateUser("inactive", oldDate);
        inactive.Status = UserStatus.Inactive;
        var neverLoggedIn = CreateUser("never", null);
        neverLoggedIn.Created = oldDate;
        var unknown = CreateUser("unknown", null);
        var recentLogin = CreateUser("recent", clock.Now.Date.AddDays(-1));
        recentLogin.Created = oldDate;
        dbContext.Users.AddRange(inactive, neverLoggedIn, unknown, recentLogin);
        await dbContext.SaveChangesAsync();

        await CreateJob(dbContext, clock).Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);

        dbContext.OutboxMessages.Single().Content.ShouldContain("never@example.com");
        dbContext.OutboxMessages.Count().ShouldBe(1);
    }

    [Test]
    public async Task Execute_ShouldQueueOncePerActivityPeriodAndRecalculateCutoff()
    {
        await using var dbContext = CreateContext();
        var clock = new TestClock();
        var user = CreateUser("user", clock.Now.Date.AddDays(-22));
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        var job = CreateJob(dbContext, clock);
        await job.Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);
        dbContext.OutboxMessages.Count().ShouldBe(0);

        clock.Now = clock.Now.AddDays(1);
        await job.Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);
        dbContext.OutboxMessages.Single().ProcessedOnUtc = clock.Now.UtcDateTime;
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        await job.Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);
        dbContext.OutboxMessages.Count().ShouldBe(1);

        user = await dbContext.Users.IgnoreAutoIncludes().SingleAsync();
        user.LastLogin = clock.Now.DateTime;
        await dbContext.SaveChangesAsync();
        clock.Now = clock.Now.AddDays(23);
        await job.Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);
        dbContext.OutboxMessages.Count().ShouldBe(2);
    }

    [Test]
    public async Task Execute_WhenCancelledAfterFirstSave_ShouldCatchUpRemainingUsersNextDay()
    {
        await using var dbContext = CreateContext();
        var clock = new TestClock();
        dbContext.Users.AddRange(CreateUser("first", clock.Now.Date.AddDays(-23)),
            CreateUser("second", clock.Now.Date.AddDays(-23)));
        await dbContext.SaveChangesAsync();
        using var cancellation = new CancellationTokenSource();
        var unitOfWork = CreateUnitOfWork(dbContext);
        unitOfWork.Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(async token =>
            {
                var count = await dbContext.SaveChangesAsync(token);
                cancellation.Cancel();
                return count;
            });
        var job = new NotifyAccountDeactivationJob(NullLogger<NotifyAccountDeactivationJob>.Instance,
            unitOfWork.Object, clock);

        await Should.ThrowAsync<OperationCanceledException>(() =>
            job.Execute(Mock.Of<IJobExecutionContext>(), cancellation.Token).AsTask());

        dbContext.ChangeTracker.Clear();
        dbContext.OutboxMessages.Count().ShouldBe(1);
        clock.Now = clock.Now.AddDays(1);
        await CreateJob(dbContext, clock).Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);
        dbContext.OutboxMessages.Count().ShouldBe(2);
    }

    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Test]
    public async Task Execute_WhenSaveFails_ShouldLeaveUserEligibleForRetry()
    {
        await using var dbContext = CreateContext();
        var clock = new TestClock();
        dbContext.Users.Add(CreateUser("user", clock.Now.Date.AddDays(-23)));
        await dbContext.SaveChangesAsync();
        var unitOfWork = CreateUnitOfWork(dbContext);
        unitOfWork.Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Save failed"));
        var job = new NotifyAccountDeactivationJob(NullLogger<NotifyAccountDeactivationJob>.Instance,
            unitOfWork.Object, clock);

        var exception = await Should.ThrowAsync<JobExecutionException>(() =>
            job.Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None).AsTask());

        exception.RefireImmediately.ShouldBeTrue();
        dbContext.ChangeTracker.Entries().ShouldBeEmpty();
        dbContext.OutboxMessages.Count().ShouldBe(0);
        await CreateJob(dbContext, clock).Execute(Mock.Of<IJobExecutionContext>(), CancellationToken.None);
        dbContext.OutboxMessages.Count().ShouldBe(1);
    }

    private static ApplicationUser CreateUser(string id, DateTime? lastLogin) => new()
    {
        Id = id,
        Email = $"{id}@example.com",
        DisplayName = id,
        CreatedBy = "test",
        ProviderId = "provider",
        TenantId = "1.",
        TenantName = "tenant",
        MemorableDate = "date",
        MemorablePlace = "place",
        Status = UserStatus.Active,
        LastLogin = lastLogin
    };

    private static Mock<IUnitOfWork> CreateUnitOfWork(ApplicationDbContext dbContext)
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(work => work.DbContext).Returns(dbContext);
        unitOfWork.Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(token => dbContext.SaveChangesAsync(token));
        return unitOfWork;
    }

    private static NotifyAccountDeactivationJob CreateJob(ApplicationDbContext dbContext, TimeProvider clock) =>
        new(NullLogger<NotifyAccountDeactivationJob>.Instance, CreateUnitOfWork(dbContext).Object, clock);
}