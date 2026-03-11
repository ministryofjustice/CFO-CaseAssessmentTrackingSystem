#nullable enable
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.ParticipantLabels.BusinessRules;

public class CannotCloseSystemLabelsWithoutForcingTests
{
    [Test]
    public void IsBroken_WhenSystemLabelAndNotForced_ShouldReturnTrue()
    {
        var label = CreateLabel(LabelScope.System);
        var rule = new CannotCloseSystemLabelsWithoutForcing(label, isForced: false);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("System labels cannot be manually closed.");
    }

    [Test]
    public void IsBroken_WhenSystemLabelAndForced_ShouldReturnFalse()
    {
        var label = CreateLabel(LabelScope.System);
        var rule = new CannotCloseSystemLabelsWithoutForcing(label, isForced: true);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenUserLabelAndNotForced_ShouldReturnFalse()
    {
        var label = CreateLabel(LabelScope.User);
        var rule = new CannotCloseSystemLabelsWithoutForcing(label, isForced: false);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenUserLabelAndForced_ShouldReturnFalse()
    {
        var label = CreateLabel(LabelScope.User);
        var rule = new CannotCloseSystemLabelsWithoutForcing(label, isForced: true);

        rule.IsBroken().ShouldBeFalse();
    }

    private Label CreateLabel(LabelScope scope)
    {
        var counter = new TestLabelCounter();
        return Label.Create(
            "Test Label",
            "Test Description",
            scope,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            counter);
    }

    private class TestLabelCounter : ILabelCounter
    {
        public int CountVisibleLabels(string name, string? contractId) => 0;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
