#nullable enable
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class LabelCannotBeNullOrEmptyRuleTests
{
    [Test]
    public void IsBroken_WhenNameIsNull_ShouldReturnTrue()
    {
        var rule = new LabelCannotBeNullOrEmptyRule(null!);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Label cannot be null or empty.");
    }

    [Test]
    public void IsBroken_WhenNameIsEmpty_ShouldReturnTrue()
    {
        var rule = new LabelCannotBeNullOrEmptyRule("");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenNameIsWhitespace_ShouldReturnTrue()
    {
        var rule = new LabelCannotBeNullOrEmptyRule("   ");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenNameHasValue_ShouldReturnFalse()
    {
        var rule = new LabelCannotBeNullOrEmptyRule("Valid Name");

        rule.IsBroken().ShouldBeFalse();
    }
}
