#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.QualityAssurance.Queries;

[TestFixture]
public class GetPqaAssigneesQueryTests
{
    private GetPqaAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetPqaAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetPqaAssignees.Query(null!);

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
        var query = new GetPqaAssignees.Query(user);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }
}
