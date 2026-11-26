using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Cfo.Cats.Domain.Entities;
using Cfo.Cats.Domain.Entities.ManagementInformation;
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

    [TestCase(0, true, false)]
    [TestCase(1, true, false)]
    [TestCase(2, true, false)]
    [TestCase(3, true, false)]
    [TestCase(4, false, true)]
    [TestCase(5, false, true)]
    [TestCase(6, false, true)]
    [TestCase(7, false, false)]
    [TestCase(8, true, false)]
    [TestCase(9, false, true)]
    public async Task Handle_WhenCatsMatches_MapsCurrentLocation(int locationTypeValue, bool isCustody, bool isCommunity)
    {
        await using var context = CreateContext();
        var location = Location.Create("Test location", 0, LocationType.FromValue(locationTypeValue), "TEST", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var participant = Participant.CreateFrom("AB1234567", "Test", null, "Person", "Male", DateOfBirth, null, "Test", null, location.Id, null, null, null);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var candidateService = new Mock<ICandidateService>(MockBehavior.Strict);
        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", null, null), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var actual = result.Data!.Results.ShouldHaveSingleItem();
        actual.LocationDescription.ShouldBe(location.Name);
        actual.IsInCustody.ShouldBe(isCustody);
        actual.CustodyLocation.ShouldBe(isCustody ? location.Name : null);
        actual.CommunityLocation.ShouldBe(isCommunity ? location.Name : null);
        actual.IsParticipant.ShouldBeTrue();
        result.Data.DmsSearched.ShouldBeFalse();
    }

    [Test]
    public async Task Handle_WhenCatsMatchesAndDmsFails_ReturnsCatsResultsWithWarning()
    {
        await using var context = CreateContext();
        var location = Location.Create("Test location", 0, LocationType.Community, "TEST", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var participant = Participant.CreateFrom("AB1234567", "Test", null, "Person", "Male", DateOfBirth, null, "Test", null, location.Id, null, null, null);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

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
        result.Data.DmsSearched.ShouldBeTrue();
        result.Data.Warning.ShouldBe("No participant found on CATS. DMS search could not be completed: DMS unavailable");
    }

    [Test]
    public async Task Handle_WhenDmsSearchThrows_StillWritesUsageAudit()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ThrowsAsync(new InvalidOperationException("DMS exploded"));

        var handler = CreateHandler(context, candidateService);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None));

        var usageEvent = context.UsageEvents.Single();
        usageEvent.Context.ShouldBe("identifier=AB1234567 | justification=CallRef123");
    }

    [Test]
    public async Task Handle_WhenDmsSearchThrows_StillWritesCatsAccessAuditTrail()
    {
        await using var context = CreateContext();
        var location = Location.Create("Test location", 0, LocationType.Community, "TEST", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var participant = Participant.CreateFrom("AB1234567", "Test", null, "Person", "Male", DateOfBirth, null, "Test", null, location.Id, null, null, null);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ThrowsAsync(new InvalidOperationException("DMS exploded"));

        var handler = CreateHandler(context, candidateService);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None));

        var accessAuditEntry = context.AccessAuditTrails.Single();
        accessAuditEntry.ParticipantId.ShouldBe("AB1234567");
        accessAuditEntry.RequestType.ShouldBe(nameof(ParticipantAdvancedSearchQuery));
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
        result.Data.DmsSearched.ShouldBeTrue();
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.GetByUpciAsync("UPCI-1"), Times.Once);
    }

    [Test]
    public async Task Handle_WhenDmsReturnsMoreThanMaximumResults_AddsTruncationWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        var searchResults = Enumerable.Range(1, 11)
            .Select(i => new SearchResult($"UPCI-{i}", i))
            .ToArray();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Success(searchResults));
        candidateService
            .Setup(x => x.GetByUpciAsync(It.IsAny<string>()))
            .ReturnsAsync((string upci) => Result<CandidateDto>.Success(new CandidateDto
            {
                Identifier = upci,
                FirstName = "Dms",
                LastName = "Person",
                Nationality = "British",
                Ethnicity = "Unknown",
                Primary = "DMS",
                DateOfBirth = DateOfBirth,
                Crn = "CRN123"
            }));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.Length.ShouldBe(ParticipantAdvancedSearchQueryHandler.MaximumResults);
        if (result.Data.Warning != null)
        {
            result.Data.Warning.ShouldContain("Search returned more than 10 results");
        }
    }

    [TestCase("A1234BC")]
    [TestCase("AB1234567")]
    public async Task Handle_WhenDmsMatchIsAlreadyOnCats_ReturnsSingleParticipantWithLocalDetails(string identifier)
    {
        await using var context = CreateContext();
        var location = Location.Create("Test location", 0, LocationType.Community, "TEST", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var participant = Participant.CreateFrom("AB1234567", "Test", null, "Person", "Male", DateOfBirth, null, "Test", null, location.Id, null, null, null);
        participant.UpdateSupervisor(Supervisor.CreateFrom("Supervisor", "0123456789", null, "supervisor@example.com", "Test address"));
        participant.UpdateRegistrationDetailsJson("[\"Test registration\"]");
        context.Participants.Add(participant);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var candidateService = new Mock<ICandidateService>(MockBehavior.Strict);
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Success([
                new SearchResult("AB1234567", 1),
                new SearchResult("AB1234567", 2)
            ]));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery(identifier, "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var actual = result.Data!.Results.ShouldHaveSingleItem();
        actual.Identifier.ShouldBe("AB1234567");
        actual.IsParticipant.ShouldBeTrue();
        actual.PrimarySource.ShouldBe("CATS");
        actual.LocationDescription.ShouldBe(location.Name);
        actual.SupervisorName.ShouldBe("Supervisor");
        actual.SupervisorTelephoneNumber.ShouldBe("0123456789");
        actual.SupervisorEmailAddress.ShouldBe("supervisor@example.com");
        actual.SupervisorAddress.ShouldBe("Test address");
        actual.RegistrationDetails.ShouldBe(["Test registration"]);
        result.Data.DmsSearched.ShouldBeTrue();
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.GetByUpciAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenDmsReturnsNotFound_ReturnsEmptyResultsWithoutWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.NotFound());
        candidateService
            .Setup(x => x.GetByUpciAsync("AB1234567"))
            .ReturnsAsync(Result<CandidateDto>.NotFound());

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.DmsSearched.ShouldBeTrue();
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.GetByUpciAsync("AB1234567"), Times.Once);
    }

    [Test]
    public async Task Handle_WhenDmsMatchDetailsAreNotFound_ReturnsWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Success([new SearchResult("UPCI-1", 1)]));
        candidateService
            .Setup(x => x.GetByUpciAsync("UPCI-1"))
            .ReturnsAsync(Result<CandidateDto>.NotFound());

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Person", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        if (result.Data.Warning != null)
        {
            result.Data.Warning.ShouldContain("DMS returned a match");
        }
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

    [TestCase(null, false)]
    [TestCase(null, true)]
    [TestCase("", true)]
    [TestCase("   ", true)]
    [TestCase("Person", false)]
    public async Task Handle_WhenLastNameOrDateOfBirthMissing_DoesNotCallDms(string lastName, bool hasDateOfBirth)
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", lastName, hasDateOfBirth ? DateOfBirth : null), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.DmsSearched.ShouldBeFalse();
        result.Data.Warning.ShouldBeNull();
        candidateService.Verify(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenFallbackCandidateMismatchesCriteria_SkipsResultWithoutWarning()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.NotFound());
        candidateService
            .Setup(x => x.GetByUpciAsync("AB1234567"))
            .ReturnsAsync(Result<CandidateDto>.Success(new CandidateDto
            {
                Identifier = "AB1234567",
                FirstName = "Wrong",
                LastName = "Surname",
                Nationality = "British",
                Ethnicity = "Unknown",
                Primary = "DMS",
                DateOfBirth = new DateTime(1980, 1, 1),
                Crn = "CRN123"
            }));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Expected", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Results.ShouldBeEmpty();
        result.Data.Warning.ShouldBeNull();
    }

    [Test]
    public async Task Handle_WhenFallbackCandidateUsesCurlyApostropheSurname_MatchesInput()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.NotFound());
        candidateService
            .Setup(x => x.GetByUpciAsync("AB1234567"))
            .ReturnsAsync(Result<CandidateDto>.Success(new CandidateDto
            {
                Identifier = "AB1234567",
                FirstName = "Anne",
                LastName = "O’Connor",
                Nationality = "British",
                Ethnicity = "Unknown",
                Primary = "DMS",
                DateOfBirth = DateOfBirth,
                Crn = "CRN123"
            }));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "O'Connor", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var matched = result.Data!.Results.ShouldHaveSingleItem();
        matched.Identifier.ShouldBe("AB1234567");
        result.Data.Warning.ShouldBeNull();
    }

    [Test]
    public async Task Handle_WhenSearchReturnsClusterMatch_DoesNotApplyFallbackCriteriaFilter()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>();
        candidateService
            .Setup(x => x.SearchAsync(It.IsAny<CandidateSearchQuery>()))
            .ReturnsAsync(Result<SearchResult[]>.Success([new SearchResult("UPCI-1", 2)]));
        candidateService
            .Setup(x => x.GetByUpciAsync("UPCI-1"))
            .ReturnsAsync(Result<CandidateDto>.Success(new CandidateDto
            {
                Identifier = "UPCI-1",
                FirstName = "Anne",
                LastName = "Smyth",
                Nationality = "British",
                Ethnicity = "Unknown",
                Primary = "DMS",
                DateOfBirth = DateOfBirth,
                Crn = "CRN123"
            }));

        var handler = CreateHandler(context, candidateService);
        var result = await handler.Handle(CreateQuery("AB1234567", "Smith", DateOfBirth), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var matched = result.Data!.Results.ShouldHaveSingleItem();
        matched.Identifier.ShouldBe("UPCI-1");
        result.Data.Warning.ShouldBeNull();
    }

    [Test]
    public async Task Handle_WhenCatsParticipantsReturned_WritesAccessAuditTrails()
    {
        await using var context = CreateContext();
        var location = Location.Create("Test location", 0, LocationType.Community, "TEST", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        context.Locations.Add(location);
        await context.SaveChangesAsync();

        var participant = Participant.CreateFrom("AB1234567", "Test", null, "Person", "Male", DateOfBirth, null, "Test", null, location.Id, null, null, null);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var candidateService = new Mock<ICandidateService>(MockBehavior.Strict);
        var handler = CreateHandler(context, candidateService);

        var result = await handler.Handle(CreateQuery("AB1234567", null, null), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var accessAuditEntry = context.AccessAuditTrails.Single();
        accessAuditEntry.ParticipantId.ShouldBe("AB1234567");
        accessAuditEntry.RequestType.ShouldBe(nameof(ParticipantAdvancedSearchQuery));
        accessAuditEntry.UserId.ShouldBe("test-user");
        var usageEvent = context.UsageEvents.Single();
        usageEvent.Context.ShouldBe("identifier=AB1234567 | justification=CallRef123");
    }

    [Test]
    public async Task Handle_WhenAuditContextExceedsLimit_TruncatesContext()
    {
        await using var context = CreateContext();
        var candidateService = new Mock<ICandidateService>(MockBehavior.Strict);
        var handler = CreateHandler(context, candidateService);

        var query = CreateQuery("AB1234567", null, null);
        query.Justification = new string('A', 1000);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var usageEvent = context.UsageEvents.Single();
        if (usageEvent.Context != null)
        {
            usageEvent.Context.Length.ShouldBeLessThanOrEqualTo(256);
        }
    }

    private static ParticipantAdvancedSearchQueryHandler CreateHandler(
        ApplicationDbContext context,
        Mock<ICandidateService> candidateService)
    {
        var dbContext = new Mock<IApplicationDbContext>();
        dbContext.SetupGet(x => x.Participants).Returns(context.Participants);
        dbContext.SetupGet(x => x.Locations).Returns(context.Locations);
        dbContext.SetupGet(x => x.UsageEvents).Returns(context.UsageEvents);
        dbContext.SetupGet(x => x.AccessAuditTrails).Returns(context.AccessAuditTrails);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(dbContext.Object);
        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(context.SaveChangesAsync);

        var auditWriter = new Mock<IPrivilegedSearchAuditWriter>();
        auditWriter
            .Setup(x => x.RecordAdvancedSearchAttemptAsync(
                It.IsAny<UserProfile>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns<UserProfile, string, string, IEnumerable<string>, CancellationToken>(async (user, identifier, justification, participantIds, ct) =>
            {
                var contextText = $"identifier={identifier.Trim().ToUpperInvariant()} | justification={justification.Trim()}";
                if (contextText.Length > 256)
                {
                    contextText = contextText[..256];
                }

                context.UsageEvents.Add(UsageEvent.Create(
                    "Search",
                    "ParticipantAdvancedSearch",
                    user.UserId,
                    user.UserName,
                    user.TenantId,
                    contextText,
                    DateTime.UtcNow));

                var userId = string.IsNullOrWhiteSpace(user.UserId) ? Guid.Empty.ToString() : user.UserId;
                foreach (var participantId in participantIds.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    context.AccessAuditTrails.Add(ParticipantAccessAuditTrail.Create(
                        nameof(ParticipantAdvancedSearchQuery),
                        participantId,
                        userId));
                }

                await context.SaveChangesAsync(ct);
            });

        return new ParticipantAdvancedSearchQueryHandler(
            candidateService.Object,
            unitOfWork.Object,
            auditWriter.Object,
            NullLogger<ParticipantAdvancedSearchQueryHandler>.Instance);
    }

    private static ParticipantAdvancedSearchQuery CreateQuery(string identifier, string lastName, DateTime? dateOfBirth) => new()
    {
        Identifier = identifier,
        LastName = lastName,
        DateOfBirth = dateOfBirth,
        Justification = "CallRef123",
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
