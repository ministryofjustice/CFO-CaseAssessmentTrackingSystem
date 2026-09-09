#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Queries;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Activities.Queries;

[TestFixture]
public class GetActivityPqaAssigneesQueryTests
{
    private GetActivityPqaAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetActivityPqaAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetActivityPqaAssignees.Query(null!);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var user = new UserProfile
        {
            UserId = "user-1",
            UserName = "test.user",
            Email = "test@example.com",
            DisplayName = "Test User",
            TenantId = "1.1."
        };
        var query = new GetActivityPqaAssignees.Query(user);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }
}
