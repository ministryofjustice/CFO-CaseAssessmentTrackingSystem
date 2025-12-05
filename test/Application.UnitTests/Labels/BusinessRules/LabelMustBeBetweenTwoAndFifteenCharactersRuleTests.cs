#nullable enable
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class LabelMustBeBetweenTwoAndFifteenCharactersRuleTests
{
    [Test]
    public void IsBroken_WhenNameHasOneCharacter_ShouldReturnTrue()
    {
        var rule = new LabelMustBeBetweenTwoAndFifteenCharactersRule("A");

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Label must be between 2 and 15 characters.");
    }

    [Test]
    public void IsBroken_WhenNameHasTwoCharacters_ShouldReturnFalse()
    {
        var rule = new LabelMustBeBetweenTwoAndFifteenCharactersRule("AB");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenNameHasFifteenCharacters_ShouldReturnFalse()
    {
        var rule = new LabelMustBeBetweenTwoAndFifteenCharactersRule("FifteenCharName");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenNameHasSixteenCharacters_ShouldReturnTrue()
    {
        var rule = new LabelMustBeBetweenTwoAndFifteenCharactersRule("SixteenCharNames");

        rule.IsBroken().ShouldBeTrue();
    }

    [TestCase("ABC")]
    [TestCase("ValidLabel")]
    [TestCase("TenCharsOk")]
    public void IsBroken_WhenNameIsWithinRange_ShouldReturnFalse(string name)
    {
        var rule = new LabelMustBeBetweenTwoAndFifteenCharactersRule(name);

        rule.IsBroken().ShouldBeFalse();
    }
}
