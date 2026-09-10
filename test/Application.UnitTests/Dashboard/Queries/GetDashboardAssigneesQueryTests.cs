#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Dashboard.Queries;

internal static class TestHelper
{
    public static UserProfile CreateUser() => new()
    {
        UserId = "user-1",
        UserName = "test.user",
        Email = "test@example.com",
        DisplayName = "Test User",
        TenantId = "1.1."
    };
}

[TestFixture]
public class GetCasesPerLocationAssigneesQueryTests
{
    private GetCasesPerLocationAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetCasesPerLocationAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetCasesPerLocationAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetCasesPerLocationAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetEducationAndTrainingAssigneesQueryTests
{
    private GetEducationAndTrainingAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetEducationAndTrainingAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetEducationAndTrainingAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetEducationAndTrainingAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetEmploymentAssigneesQueryTests
{
    private GetEmploymentAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetEmploymentAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetEmploymentAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetEmploymentAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetEnrolmentAssigneesQueryTests
{
    private GetEnrolmentAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetEnrolmentAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetEnrolmentAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetEnrolmentAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetInductionAssigneesQueryTests
{
    private GetInductionAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetInductionAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetInductionAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetInductionAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetInitiativeObjectiveAssigneesQueryTests
{
    private GetInitiativeObjectiveAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetInitiativeObjectiveAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetInitiativeObjectiveAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetInitiativeObjectiveAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetPaidActivityAssigneesQueryTests
{
    private GetPaidActivityAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetPaidActivityAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetPaidActivityAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetPaidActivityAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetPathwayPlanAssigneesQueryTests
{
    private GetPathwayPlanAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetPathwayPlanAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetPathwayPlanAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetPathwayPlanAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetReassessmentAssigneesQueryTests
{
    private GetReassessmentAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetReassessmentAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetReassessmentAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetReassessmentAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetRecentlyApprovedActivityAssigneesQueryTests
{
    private GetRecentlyApprovedActivityAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetRecentlyApprovedActivityAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetRecentlyApprovedActivityAssignees.Query(null!)
        {
            StartDate = default,
            EndDate = default
        };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetRecentlyApprovedActivityAssignees.Query(TestHelper.CreateUser())
        {
            TenantId = "1.1.",
            StartDate = default,
            EndDate = default
        };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}

[TestFixture]
public class GetSupportReferralAssigneesQueryTests
{
    private GetSupportReferralAssignees.Validator _validator = null!;

    [SetUp]
    public void Setup() => _validator = new GetSupportReferralAssignees.Validator();

    [Test]
    public void Validator_WhenCurrentUserIsNull_ShouldHaveError()
    {
        var query = new GetSupportReferralAssignees.Query(null!);
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CurrentUser");
    }

    [Test]
    public void Validator_WhenCurrentUserIsValid_ShouldBeValid()
    {
        var query = new GetSupportReferralAssignees.Query(TestHelper.CreateUser()) { TenantId = "1.1." };
        var result = _validator.Validate(query);
        result.IsValid.ShouldBeTrue();
    }
}
