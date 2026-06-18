#nullable enable
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants.BusinessRules;

public class EnrolmentStatusTransitionMustBeValidTests
{
    [Test]
    public void IsBroken_WhenTransitionIsAllowed_ShouldReturnFalse()
    {
        var rule = new EnrolmentStatusTransitionMustBeValid(
            EnrolmentStatus.IdentifiedStatus,
            EnrolmentStatus.EnrollingStatus);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenTransitionIsNotAllowed_ShouldReturnTrue()
    {
        var rule = new EnrolmentStatusTransitionMustBeValid(
            EnrolmentStatus.IdentifiedStatus,
            EnrolmentStatus.ApprovedStatus);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Participants cannot transition from Identified to Approved");
    }
}
