using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Pipeline.ValidationSpecifications;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants.Queries;

public class SearchParticipantsTests
{
    private ApplicationDbContext _db = null!;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new ApplicationDbContext(options);

        _db.Locations.Add(Location.Create("Test Wing", GenderProvision.Any.Value, LocationType.Wing, "C001", DateTime.Today.AddYears(-1), DateTime.Today.AddYears(1)));
        await _db.SaveChangesAsync();

        _db.Participants.AddRange(
            Create("P000000001", "Mary Ann", "Smith"),
            Create("P000000002", "John", "Smith"),
            Create("P000000003", "John", "Brown"),
            Create("P000000004", "Anna", "Van Der Berg"));
        await _db.SaveChangesAsync();
    }

    [TearDown]
    public async Task TearDown() => await _db.DisposeAsync();

    private static Participant Create(string id, string first, string last) =>
        Participant.CreateFrom(id, first, null, last, null, DateTime.UtcNow.AddYears(-30), null, "Test", null, 1, null, null, null);

    private SearchParticipants.Handler CreateHandler(AccessGrant grant)
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.DbContext).Returns(_db);
        return new SearchParticipants.Handler(unitOfWork.Object, [new FixedSpecification(grant)]);
    }

    private async Task<List<string>> SearchAsync(string keyword, int max = 10)
    {
        var result = await CreateHandler(AccessGrant.Granted)
            .Handle(new SearchParticipants.Query { Keyword = keyword, MaxResults = max }, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        return result.Data!.Select(r => r.Id).ToList();
    }

    [TestCase("Mary Ann Smith", "P000000001")]
    [TestCase("John  Smith", "P000000002")]
    [TestCase("  John Brown  ", "P000000003")]
    [TestCase("Van Der Berg", "P000000004")]
    [TestCase("Smith Mary", "P000000001")]
    [TestCase("P000000003", "P000000003")]
    public async Task Name_and_identifier_searches_should_find_the_participant(string keyword, string expectedId)
    {
        var ids = await SearchAsync(keyword);

        ids.ShouldBe([expectedId]);
    }

    [Test]
    public async Task Single_word_should_match_every_participant_with_it()
    {
        var ids = await SearchAsync("Smith");

        ids.ShouldBe(["P000000001", "P000000002"], ignoreOrder: true);
    }

    [Test]
    public async Task Blank_keyword_should_return_nothing() =>
        (await SearchAsync("   ")).ShouldBeEmpty();

    [Test]
    public async Task Results_should_be_limited_to_max_results() =>
        (await SearchAsync("P00000000", max: 2)).Count.ShouldBe(2);

    [TestCase(AccessGrant.Granted, true)]
    [TestCase(AccessGrant.NotGranted, false)]
    [TestCase(AccessGrant.Denied, false)]
    public async Task Participants_should_be_found_regardless_of_access_and_flag_it(AccessGrant grant, bool expectedAccess)
    {
        var result = await CreateHandler(grant)
            .Handle(new SearchParticipants.Query { Keyword = "Brown" }, CancellationToken.None);

        var found = result.Data!.ShouldHaveSingleItem();
        found.Id.ShouldBe("P000000003");
        found.HasAccess.ShouldBe(expectedAccess);
    }

    private sealed class FixedSpecification(AccessGrant grant) : AccessValidationSpecification
    {
        public override int Order => 1;

        protected override Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken)
            => Task.FromResult(grant);
    }
}
