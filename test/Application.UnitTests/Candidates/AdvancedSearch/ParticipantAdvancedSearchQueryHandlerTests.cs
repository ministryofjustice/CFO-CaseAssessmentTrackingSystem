using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Candidates.AdvancedSearch;

public class ParticipantAdvancedSearchQueryHandlerTests
{
    private static readonly DateTime DateOfBirth = new(1990, 1, 1);

    [Test]
    public async Task Handle_WhenCatsMatchesAndDmsFails_ReturnsCatsResultsWithWarning()
    {
        await using var context = CreateContext();
        var location = Location.Create("Test location", 0, LocationType.Community, "TEST", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        var participant = Participant.CreateFrom("AB1234567", "Test", null, "Person", "Male", DateOfBirth, null, "Test", null, 1, null, null, null);
        context.Locations.Add(location);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Failure("DMS unavailable"));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.Length.ShouldBe(1);
        result.Data.Warning!.ShouldContain("DMS search could not be completed");
        candidateService.Verify(x => x.SearchAsync(It.Is<CandidateSearchQuery>(q => q.ExternalIdentifier == "AB1234567")), Times.Once);
    }

    [Test]
    public async Task Handle_WhenNoCatsMatchAndDmsFails_ReturnsEmptyResultsWithWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Failure("DMS unavailable"));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.Warning.ShouldBe("No participant found on CATS. DMS search could not be completed: DMS unavailable");
    }

    [Test]
    public async Task Handle_WhenDmsMatches_ReturnsMergedCatsAndDmsResults()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Success([new SearchResult("UPCI-1", 1)]));
        candidateService
            .Setup(x => x.GetByUpciAsync("UPCI-1"))
            .ReturnsAsync(Result<CandidateDto>.Success(new CandidateDto
            {
                Identifier = "CATS-2",
                FirstName = "Dms",
                LastName = "Candidate",
                Nationality = "British",
                Ethnicity = "Unknown",
                Primary = "DMS",
                DateOfBirth = new DateTime(1985, 5, 5),
                Crn = "CRN123"
            }));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Candidate", new DateTime(1985, 5, 5)), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.Length.ShouldBe(1);
        result.Data.Results[0].Identifier.ShouldBe("CATS-2");
        result.Data.Results[0].PrimarySource.ShouldBe("DMS");
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.GetByUpciAsync("UPCI-1"), Times.Once);
    }

    [Test]
    public async Task Handle_WhenDmsReturnsNotFound_ReturnsEmptyResultsWithoutWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.NotFound());

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.GetByUpciAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenDmsDetailsFail_ReturnsWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Success([new SearchResult("UPCI-1", 1)]));
        candidateService
            .Setup(x => x.GetByUpciAsync("UPCI-1"))
            .ReturnsAsync(Result<CandidateDto>.Failure("DMS details unavailable"));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.Warning!.ShouldContain("DMS details could not be loaded");
    }

    [Test]
    public async Task Handle_WhenLastNameOrDateOfBirthMissing_DoesNotCallDms()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", null, null), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()), Times.Never);
    }

    private static ParticipantAdvancedSearchQueryHandler CreateHandler(
        ApplicationDbContext context,
        Mock<ICandidateService> candidateService)
    {
        var dbContext = new Mock<IApplicationDbContext>();
        dbContext.SetupGet(x => x.Participants).Returns(context.Participants);
        dbContext.SetupGet(x => x.Locations).Returns(context.Locations);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(dbContext.Object);

        return new ParticipantAdvancedSearchQueryHandler(
            candidateService.Object,
            unitOfWork.Object,
            NullLogger<ParticipantAdvancedSearchQueryHandler>.Instance);
    }

    private static ParticipantAdvancedSearchQuery CreateQuery(string identifier, string lastName, DateTime? dateOfBirth) => new()
    {
        Identifier = identifier,
        LastName = lastName,
        DateOfBirth = dateOfBirth,
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
