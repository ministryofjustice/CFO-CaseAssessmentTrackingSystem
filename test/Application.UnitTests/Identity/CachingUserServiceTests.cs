using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Infrastructure.Services.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Application.UnitTests.Identity;

public class CachingUserServiceTests
{
    private Mock<IUserService> _inner = null!;
    private FusionCache _cache = null!;
    private CachingUserService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _inner = new Mock<IUserService>();
        _inner.Setup(s => s.Summaries).Returns(new List<ApplicationUserSummaryDto>
        {
            new() { Id = "user-1", UserName = "jane", DisplayName = "Jane Doe", TenantId = "1.1." },
            new() { Id = "user-2", UserName = "john", DisplayName = "John Smith", TenantId = "1.2." }
        });

        _cache = new FusionCache(new FusionCacheOptions());
        _sut = new CachingUserService(_cache, _inner.Object, NullLogger<CachingUserService>.Instance);
    }

    [TearDown]
    public void TearDown() => _cache.Dispose();

    [Test]
    public void GetDisplayName_ReturnsNameForKnownUser() =>
        _sut.GetDisplayName("user-2").ShouldBe("John Smith");

    [Test]
    public void GetDisplayName_ReturnsNullForUnknownOrNullUser()
    {
        _sut.GetDisplayName("missing").ShouldBeNull();
        _sut.GetDisplayName(null).ShouldBeNull();
    }

    [Test]
    public void GetDisplayName_LoadsSummariesOnceAcrossManyLookups()
    {
        for (var i = 0; i < 50; i++)
        {
            _sut.GetDisplayName("user-1");
        }

        _inner.Verify(s => s.Summaries, Times.Once);
    }

    [Test]
    public void Refresh_ClearsCachedSummariesAndDisplayNames()
    {
        _sut.GetDisplayName("user-1");

        _sut.Refresh();
        _sut.GetDisplayName("user-1");

        _inner.Verify(s => s.Summaries, Times.Exactly(2));
    }
}
