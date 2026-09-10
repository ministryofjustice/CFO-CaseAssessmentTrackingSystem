#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Queries;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants.Queries;

[TestFixture]
public class GetEngagementAssigneesQueryTests
{
    private GetEngagementAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetEngagementAssignees.Validator();

    private static UserProfile CreateUser() => new()
    {
        UserId = "user-1",
        UserName = "test.user",
        Email = "test@example.com",
        DisplayName = "Test User",
        TenantId = "1.1."
    };

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetEngagementAssignees.Query(null!);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetEngagementAssignees.Query(CreateUser())
        {
            TenantId = "1.1.",
            JustMyCases = true,
            LocationId = 42,
            EngagementType = "Support",
            HideRecentEngagements = true
        };

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Query_DefaultsToNoAdditionalFilters()
    {
        var query = new GetEngagementAssignees.Query(CreateUser());

        query.TenantId.ShouldBeNull();
        query.LocationId.ShouldBeNull();
        query.EngagementType.ShouldBeNull();
        query.JustMyCases.ShouldBeFalse();
        query.HideRecentEngagements.ShouldBeFalse();
    }
}
