#nullable enable
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class LabelNameMustBeValidLengthTests
{
    [Test]
    public void IsBroken_WhenNameHasOneCharacter_ShouldReturnTrue()
    {
        var rule = new LabelNameMustBeValidLength("A");

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Label must be between 2 and 25 characters.");
    }

    [Test]
    public void IsBroken_WhenNameHasTwoCharacters_ShouldReturnFalse()
    {
        var rule = new LabelNameMustBeValidLength("AB");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenNameHasFifteenCharacters_ShouldReturnFalse()
    {
        var rule = new LabelNameMustBeValidLength("FifteenCharName");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenNameHasTwentySixCharacters_ShouldReturnTrue()
    {
        var twentySix = new string('x', 26);
        var rule = new LabelNameMustBeValidLength(twentySix);

        rule.IsBroken().ShouldBeTrue();
    }

    [TestCase("ABC")]
    [TestCase("ValidLabel")]
    [TestCase("TenCharsOk")]
    public void IsBroken_WhenNameIsWithinRange_ShouldReturnFalse(string name)
    {
        var rule = new LabelNameMustBeValidLength(name);

        rule.IsBroken().ShouldBeFalse();
    }
}
