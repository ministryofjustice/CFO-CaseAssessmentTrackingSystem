#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Queries;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants.Queries;

[TestFixture]
public class GetParticipantAssigneesQueryTests
{
    private GetParticipantAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetParticipantAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetParticipantAssignees.Query(null!);

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
        var query = new GetParticipantAssignees.Query(user);

        var result = _validator.Validate(query);

        result.IsValid.ShouldBeTrue();
    }
}
