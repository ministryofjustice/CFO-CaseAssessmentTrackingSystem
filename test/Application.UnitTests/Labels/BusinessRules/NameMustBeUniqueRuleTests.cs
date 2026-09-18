#nullable enable
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class NameMustBeUniqueRuleTests
{
    [Test]
    public void IsBroken_WhenDuplicateNameExists_ShouldReturnTrue()
    {
        var labelCounter = new TestLabelCounter(1);
        var rule = new NameMustBeUniqueRule(labelCounter, "Duplicate");

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Label names must be unique");
    }

    [Test]
    public void IsBroken_WhenNameIsUnique_ShouldReturnFalse()
    {
        var labelCounter = new TestLabelCounter(0);
        var rule = new NameMustBeUniqueRule(labelCounter, "Unique");

        rule.IsBroken().ShouldBeFalse();
    }

    private class TestLabelCounter : ILabelCounter
    {
        private readonly int _visibleCount;

        public TestLabelCounter(int visibleCount)
        {
            _visibleCount = visibleCount;
        }

        public int CountLabelsWithName(string name) => _visibleCount;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
