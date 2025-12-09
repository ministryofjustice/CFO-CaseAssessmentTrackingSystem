#nullable enable
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class LabelDescriptionMustBeValidLengthTests
{
    [Test]
    public void IsBroken_WhenDescriptionHasTwoCharacters_ShouldReturnTrue()
    {
        var rule = new LabelDescriptionMustBeValidLength("AB");

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Description must be between 3 and 200 characters.");
    }

    [Test]
    public void IsBroken_WhenDescriptionHasThreeCharacters_ShouldReturnFalse()
    {
        var rule = new LabelDescriptionMustBeValidLength("ABC");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenDescriptionHasTwoHundredCharacters_ShouldReturnFalse()
    {
        var description = new string('A', 200);
        var rule = new LabelDescriptionMustBeValidLength(description);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenDescriptionHasTwoHundredAndOneCharacters_ShouldReturnTrue()
    {
        var description = new string('A', 201);
        var rule = new LabelDescriptionMustBeValidLength(description);

        rule.IsBroken().ShouldBeTrue();
    }

    [TestCase("Valid")]
    [TestCase("A valid description")]
    [TestCase("This is a longer description that contains multiple words and still falls within the valid range")]
    public void IsBroken_WhenDescriptionIsWithinRange_ShouldReturnFalse(string description)
    {
        var rule = new LabelDescriptionMustBeValidLength(description);

        rule.IsBroken().ShouldBeFalse();
    }
}
