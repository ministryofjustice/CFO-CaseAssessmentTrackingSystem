#nullable enable
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Activities.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Activities.BusinessRules;

public class ActivityStatusTransitionMustBeValidTests
{
    [Test]
    public void IsBroken_WhenTransitionIsAllowed_ShouldReturnFalse()
    {
        var rule = new ActivityStatusTransitionMustBeValid(
            ActivityStatus.PendingStatus,
            ActivityStatus.SubmittedToProviderStatus);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenTransitionIsNotAllowed_ShouldReturnTrue()
    {
        var rule = new ActivityStatusTransitionMustBeValid(
            ActivityStatus.ApprovedStatus,
            ActivityStatus.SubmittedToProviderStatus);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Activities cannot transition from Approved to Submitted to Provider");
    }
}
