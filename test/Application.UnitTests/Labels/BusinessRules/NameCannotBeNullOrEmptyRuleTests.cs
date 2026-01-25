#nullable enable
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class NameCannotBeNullOrEmptyRuleTests
{
    [Test]
    public void IsBroken_WhenNameIsNull_ShouldReturnTrue()
    {
        var rule = new NameCannotBeNullOrEmptyRule(null!);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Label Name cannot be null or empty.");
    }

    [Test]
    public void IsBroken_WhenNameIsEmpty_ShouldReturnTrue()
    {
        var rule = new NameCannotBeNullOrEmptyRule("");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenNameIsWhitespace_ShouldReturnTrue()
    {
        var rule = new NameCannotBeNullOrEmptyRule("   ");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenNameHasValue_ShouldReturnFalse()
    {
        var rule = new NameCannotBeNullOrEmptyRule("Valid Name");

        rule.IsBroken().ShouldBeFalse();
    }
}
