using System.Text.Json;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Infrastructure.Jobs;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Quartz;
using Cfo.Cats.Application.Messaging;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Jobs;

public class PublishOutboxMessagesJobTests
{
    [Test]
    public async Task Execute_WhenCancelledAfterPublish_ShouldSavePublishedMessageAndStopBatch()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new ApplicationDbContext(options);

        var messages = new[]
        {
            CreateMessage(DateTime.UtcNow.AddMinutes(-2)),
            CreateMessage(DateTime.UtcNow.AddMinutes(-1))
        };
        dbContext.OutboxMessages.AddRange(messages);
        await dbContext.SaveChangesAsync();

        using var cancellation = new CancellationTokenSource();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(work => work.DbContext).Returns(dbContext);
        unitOfWork
            .Setup(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(token => dbContext.SaveChangesAsync(token));

        var publisher = new Mock<IIntegrationEventPublisher>();
        publisher.Setup(p => p.PublishAsync(It.IsAny<IIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Callback<IIntegrationEvent, CancellationToken>((_, _) => cancellation.Cancel())
            .Returns(Task.CompletedTask);

        var job = new PublishOutboxMessagesJob(
            unitOfWork.Object,
            NullLogger<PublishOutboxMessagesJob>.Instance,
            publisher.Object);

        await job.Execute(Mock.Of<IJobExecutionContext>(), cancellation.Token);

        messages[0].ProcessedOnUtc.ShouldNotBeNull();
        messages[1].ProcessedOnUtc.ShouldBeNull();
        unitOfWork.Verify(work => work.SaveChangesAsync(CancellationToken.None), Times.Once);
        publisher.Verify(p => p.PublishAsync(It.IsAny<IIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static OutboxMessage CreateMessage(DateTime occurredOnUtc)
    {
        var integrationEvent = new ParticipantCreatedIntegrationEvent("P1", null, occurredOnUtc);

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(ParticipantCreatedIntegrationEvent).FullName!,
            Content = JsonSerializer.Serialize(integrationEvent),
            OccurredOnUtc = occurredOnUtc
        };
    }
}
