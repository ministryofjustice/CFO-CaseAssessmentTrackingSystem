using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Services;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants;

public class ReturnedParticipantOwnerResolverTests
{
    private const string ParticipantId = "P000000001";
    private const string OwnerId = "owner-1";
    private static readonly DateTime Now = new(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);

    private ApplicationDbContext _context = default!;
    private Mock<ILocationService> _locationService = default!;
    private Location _original = default!;
    private Location _temporary = default!;
    private ReturnedParticipantOwnerResolver _resolver = default!;

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _original = Location.Create("Original", GenderProvision.Male.Value, LocationType.Wing, "contract", Now.AddYears(-1), Now.AddYears(1));
        _temporary = Location.Create("Temporary", GenderProvision.Male.Value, LocationType.Wing, "contract", Now.AddYears(-1), Now.AddYears(1));
        _context.Locations.AddRange(_original, _temporary);
        _context.Users.Add(new ApplicationUser
        {
            Id = OwnerId,
            TenantId = "1.",
            Status = UserStatus.Active,
            CreatedBy = "test",
            DisplayName = "Owner",
            MemorableDate = "x",
            MemorablePlace = "x",
            ProviderId = "x",
            TenantName = "x"
        });
        await _context.SaveChangesAsync();

        _locationService = new Mock<ILocationService>();
        SetVisibleLocations(_original);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.DbContext).Returns(_context);

        var dateTime = new Mock<IDateTime>();
        dateTime.SetupGet(d => d.Now).Returns(Now);

        _resolver = new ReturnedParticipantOwnerResolver(unitOfWork.Object, _locationService.Object, dateTime.Object);
    }

    [TearDown]
    public async Task TearDown() => await _context.DisposeAsync();

    [Test]
    public async Task ReturnWithinWindow_RestoresPreviousOwner()
    {
        await GivenLeftOriginalAt(Now - ReturnedParticipantOwnerResolver.ReturnWindow + TimeSpan.FromDays(1));

        var owner = await _resolver.GetOwnerToRestore(ParticipantId, _temporary, _original, CancellationToken.None);

        owner.ShouldBe(OwnerId);
    }

    [Test]
    public async Task ReturnOutsideWindow_DoesNotRestore()
    {
        await GivenLeftOriginalAt(Now - ReturnedParticipantOwnerResolver.ReturnWindow - TimeSpan.FromDays(1));

        var owner = await _resolver.GetOwnerToRestore(ParticipantId, _temporary, _original, CancellationToken.None);

        owner.ShouldBeNull();
    }

    [Test]
    public async Task ReturnToCommunityLocationAfterCustody_RestoresPreviousOwner()
    {
        var community = Location.Create("Community", GenderProvision.Male.Value, LocationType.Community, "contract", Now.AddYears(-1), Now.AddYears(1));
        _context.Locations.Add(community);
        await _context.SaveChangesAsync();
        SetVisibleLocations(community);
        await GivenLeftLocationAt(community, Now.AddDays(-2), _temporary);

        var owner = await _resolver.GetOwnerToRestore(ParticipantId, _temporary, community, CancellationToken.None);

        owner.ShouldBe(OwnerId);
    }

    [Test]
    public async Task PreviousOwnerInactive_DoesNotRestore()
    {
        await GivenLeftOriginalAt(Now.AddDays(-1));
        var user = await _context.Users.FindAsync(OwnerId);
        user!.Status = UserStatus.Left;
        await _context.SaveChangesAsync();

        var owner = await _resolver.GetOwnerToRestore(ParticipantId, _temporary, _original, CancellationToken.None);

        owner.ShouldBeNull();
    }

    [Test]
    public async Task PreviousOwnerCannotSeeLocation_DoesNotRestore()
    {
        await GivenLeftOriginalAt(Now.AddDays(-1));
        SetVisibleLocations();

        var owner = await _resolver.GetOwnerToRestore(ParticipantId, _temporary, _original, CancellationToken.None);

        owner.ShouldBeNull();
    }

    [Test]
    public async Task OwnerWasNotUnassignedOnDeparture_DoesNotRestore()
    {
        await GivenLeftOriginalAt(Now.AddDays(-1), ownershipClosedAt: Now.AddDays(-10));

        var owner = await _resolver.GetOwnerToRestore(ParticipantId, _temporary, _original, CancellationToken.None);

        owner.ShouldBeNull();
    }

    private void SetVisibleLocations(params Location[] visible) => _locationService
        .Setup(s => s.GetVisibleLocations(It.IsAny<string>()))
        .Returns(visible.Select(l => new LocationDto
        {
            Id = l.Id,
            Name = l.Name,
            ContractName = "contract",
            GenderProvision = l.GenderProvision,
            LocationType = l.LocationType
        }).ToList());

    // Participant was at the original location, was owned by OwnerId, then left (and was unassigned) at leftAt.
    private async Task GivenLeftOriginalAt(DateTime leftAt, Location via = null!, DateTime ownershipClosedAt = default) =>
        await GivenLeftLocationAt(_original, leftAt, via, ownershipClosedAt);

    private async Task GivenLeftLocationAt(Location origin, DateTime leftAt, Location via = null!, DateTime ownershipClosedAt = default)
    {
        _context.ParticipantLocationHistories.AddRange(
            ParticipantLocationHistory.Create(ParticipantId, origin.Id, leftAt.AddDays(-30)).WithTo(leftAt),
            ParticipantLocationHistory.Create(ParticipantId, (via ?? _temporary).Id, leftAt));

        var ownership = ParticipantOwnershipHistory.Create(ParticipantId, OwnerId, "1.", "contract", leftAt.AddDays(-30));
        ownership.SetTo(ownershipClosedAt == default ? leftAt : ownershipClosedAt);
        _context.ParticipantOwnershipHistories.Add(ownership);
        _context.ParticipantOwnershipHistories.Add(ParticipantOwnershipHistory.Create(ParticipantId, null, null, "contract", leftAt));

        await _context.SaveChangesAsync();
    }
}
