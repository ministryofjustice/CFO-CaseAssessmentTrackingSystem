using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class DescriptionCannotBeNullOrEmptyTests
{
    [Test]
    public void IsBroken_WhenDescriptionIsNull_ShouldReturnTrue()
    {
        var rule = new DescriptionCannotBeNullOrEmpty(null!);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Description cannot be null or empty.");
    }

    [Test]
    public void IsBroken_WhenDescriptionIsEmpty_ShouldReturnTrue()
    {
        var rule = new DescriptionCannotBeNullOrEmpty("");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenDescriptionIsWhitespace_ShouldReturnTrue()
    {
        var rule = new DescriptionCannotBeNullOrEmpty("   ");

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenDescriptionHasValue_ShouldReturnFalse()
    {
        var rule = new DescriptionCannotBeNullOrEmpty("Valid description");

        rule.IsBroken().ShouldBeFalse();
    }
}
