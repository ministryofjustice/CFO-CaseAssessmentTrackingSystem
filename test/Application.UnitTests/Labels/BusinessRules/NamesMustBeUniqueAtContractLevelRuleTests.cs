#nullable enable
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class NamesMustBeUniqueAtContractLevelRuleTests
{
    [Test]
    public void IsBroken_WhenDuplicateNameExistsInSameContract_ShouldReturnTrue()
    {
        var labelCounter = new TestLabelCounter(1);
        var rule = new NamesMustBeUniqueAtContractLevelRule(
            labelCounter,
            "Duplicate",
            "CONTRACT-001");

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Labels must be unique at a contract level");
    }

    [Test]
    public void IsBroken_WhenDuplicateNameExistsInDifferentContract_ShouldReturnFalse()
    {
        var labelCounter = new TestLabelCounter(0);
        var rule = new NamesMustBeUniqueAtContractLevelRule(
            labelCounter,
            "SameName",
            "CONTRACT-002");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenNameIsUnique_ShouldReturnFalse()
    {
        var labelCounter = new TestLabelCounter(0);
        var rule = new NamesMustBeUniqueAtContractLevelRule(
            labelCounter,
            "Unique",
            "CONTRACT-001");

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenGlobalLabelNameExists_ShouldReturnTrue()
    {
        var labelCounter = new TestLabelCounter(1);
        var rule = new NamesMustBeUniqueAtContractLevelRule(
            labelCounter,
            "GlobalLabel",
            null);

        rule.IsBroken().ShouldBeTrue();
    }

    private class TestLabelCounter : ILabelCounter
    {
        private readonly int _visibleCount;

        public TestLabelCounter(int visibleCount)
        {
            _visibleCount = visibleCount;
        }

        public int CountVisibleLabels(string name, string? contractId) => _visibleCount;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
