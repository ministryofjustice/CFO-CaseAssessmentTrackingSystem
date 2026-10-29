using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Identity;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Mappings;

public class ApplicationUserSummaryDtoTests
{
    private static readonly IMapper Mapper = new MapperConfiguration(cfg =>
        cfg.AddMaps(typeof(ApplicationUserSummaryDto).Assembly)).CreateMapper();

    private static ApplicationUser CreateUser(string id = "user-1") => new()
    {
        Id = id,
        UserName = "jane.doe@example.com",
        DisplayName = "Jane Doe",
        TenantId = "1.1.1."
    };

    [Test]
    public void Map_CopiesIdUserNameDisplayNameAndTenantId()
    {
        var dto = Mapper.Map<ApplicationUserSummaryDto>(CreateUser());

        dto.Id.ShouldBe("user-1");
        dto.UserName.ShouldBe("jane.doe@example.com");
        dto.DisplayName.ShouldBe("Jane Doe");
        dto.TenantId.ShouldBe("1.1.1.");
    }

    [Test]
    public void ProjectTo_CopiesIdUserNameDisplayNameAndTenantId()
    {
        var dto = new[] { CreateUser() }
            .AsQueryable()
            .ProjectTo<ApplicationUserSummaryDto>(Mapper.ConfigurationProvider)
            .Single();

        dto.Id.ShouldBe("user-1");
        dto.UserName.ShouldBe("jane.doe@example.com");
        dto.DisplayName.ShouldBe("Jane Doe");
        dto.TenantId.ShouldBe("1.1.1.");
    }

    [Test]
    public void IdComparer_TreatsDifferentInstancesWithSameIdAsEqual()
    {
        var a = new ApplicationUserSummaryDto { Id = "user-1", DisplayName = "Jane" };
        var b = new ApplicationUserSummaryDto { Id = "user-1", DisplayName = "Jane (renamed)" };

        ApplicationUserSummaryDto.IdComparer.Equals(a, b).ShouldBeTrue();
        ApplicationUserSummaryDto.IdComparer.GetHashCode(a)
            .ShouldBe(ApplicationUserSummaryDto.IdComparer.GetHashCode(b));
    }

    [Test]
    public void IdComparer_TreatsDifferentIdsAndNullAsNotEqual()
    {
        var a = new ApplicationUserSummaryDto { Id = "user-1" };
        var b = new ApplicationUserSummaryDto { Id = "user-2" };

        ApplicationUserSummaryDto.IdComparer.Equals(a, b).ShouldBeFalse();
        ApplicationUserSummaryDto.IdComparer.Equals(a, null).ShouldBeFalse();
        ApplicationUserSummaryDto.IdComparer.Equals(null, null).ShouldBeTrue();
    }
}
