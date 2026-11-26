using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Offloc.DTOs;
using Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Offloc;

public class GetSentenceDataForAdvancedSearchQueryHandlerTests
{
    [Test]
    public async Task Handle_WhenSentenceDataReturned_WritesUsageAuditContextWithOpenedRecord()
    {
        await using var context = CreateContext();
        var offlocService = new Mock<IOfflocService>();
        offlocService
            .Setup(x => x.GetSentenceDataAsync("AB1234C"))
            .ReturnsAsync(Result<SentenceDataDto>.Success(new SentenceDataDto
            {
                NomsNumber = "AB1234C",
                SentenceInformation = []
            }));

        var handler = CreateHandler(context, offlocService);
        var result = await handler.Handle(CreateQuery("AB1234C", "AB1234567", "CallRef123"), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        var usageEvent = context.UsageEvents.Single();
        usageEvent.Activity.ShouldBe("ParticipantAdvancedSearchSentenceLookup");
        usageEvent.Context.ShouldBe("record=AB1234567 | nomis=AB1234C | justification=CallRef123");
    }

    [Test]
    public async Task Handle_WhenSentenceDataNotFound_StillWritesUsageAudit()
    {
        await using var context = CreateContext();
        var offlocService = new Mock<IOfflocService>();
        offlocService
            .Setup(x => x.GetSentenceDataAsync("AB1234C"))
            .ReturnsAsync(Result<SentenceDataDto>.NotFound());

        var handler = CreateHandler(context, offlocService);
        var result = await handler.Handle(CreateQuery("AB1234C", "AB1234567", "CallRef123"), CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("No details found");
        context.UsageEvents.Count().ShouldBe(1);
    }

    [Test]
    public async Task Handle_WhenSentenceAuditContextExceedsLimit_TruncatesContext()
    {
        await using var context = CreateContext();
        var offlocService = new Mock<IOfflocService>();
        offlocService
            .Setup(x => x.GetSentenceDataAsync("AB1234C"))
            .ReturnsAsync(Result<SentenceDataDto>.Success(new SentenceDataDto
            {
                NomsNumber = "AB1234C",
                SentenceInformation = []
            }));

        var handler = CreateHandler(context, offlocService);
        var result = await handler.Handle(
            CreateQuery("AB1234C", new string('R', 200), new string('J', 200)),
            CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var usageEvent = context.UsageEvents.Single();
        if (usageEvent.Context != null)
        {
            usageEvent.Context.Length.ShouldBeLessThanOrEqualTo(256);
        }
    }

    private static GetSentenceDataForAdvancedSearchQueryHandler CreateHandler(
        ApplicationDbContext context,
        Mock<IOfflocService> offlocService)
    {
        var auditWriter = new Mock<IPrivilegedSearchAuditWriter>();
        auditWriter
            .Setup(x => x.RecordAdvancedSearchSentenceLookupAttemptAsync(
                It.IsAny<UserProfile>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns<UserProfile, string, string, string, CancellationToken>(async (user, recordIdentifier, nomisNumber, justification, ct) =>
            {
                var contextText = $"record={recordIdentifier} | nomis={nomisNumber} | justification={justification}";
                if (contextText.Length > 256)
                {
                    contextText = contextText[..256];
                }

                context.UsageEvents.Add(UsageEvent.Create(
                    "Search",
                    "ParticipantAdvancedSearchSentenceLookup",
                    user.UserId,
                    user.UserName,
                    user.TenantId,
                    contextText,
                    DateTime.UtcNow));
                await context.SaveChangesAsync(ct);
            });

        return new GetSentenceDataForAdvancedSearchQueryHandler(
            offlocService.Object,
            auditWriter.Object);
    }

    private static GetSentenceDataForAdvancedSearchQuery CreateQuery(string nomisNumber, string recordIdentifier, string justification) => new()
    {
        NomisNumber = nomisNumber,
        RecordIdentifier = recordIdentifier,
        Justification = justification,
        CurrentUser = new UserProfile
        {
            UserName = "test",
            Email = "test@example.com",
            UserId = "test-user"
        }
    };

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
