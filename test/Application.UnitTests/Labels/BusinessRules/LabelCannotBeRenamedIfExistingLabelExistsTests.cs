#nullable enable
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class LabelCannotBeRenamedIfExistingLabelExistsTests
{
    [Test]
    public void IsBroken_WhenNewNameAlreadyExists_ShouldReturnTrue()
    {
        var labelCounter = new TestLabelCounter(1);
        var rule = new LabelCannotBeRenamedIfExistingLabelExists(
            "NewName",
            "OldName",
            "CONTRACT-001",
            labelCounter);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Cannot rename label from OldName to NewName as the label already exists.");
    }

    [Test]
    public void IsBroken_WhenNewNameDoesNotExist_ShouldReturnFalse()
    {
        var labelCounter = new TestLabelCounter(0);
        var rule = new LabelCannotBeRenamedIfExistingLabelExists(
            "NewName",
            "OldName",
            "CONTRACT-001",
            labelCounter);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenRenamingGlobalLabelAndNewNameExists_ShouldReturnTrue()
    {
        var labelCounter = new TestLabelCounter(1);
        var rule = new LabelCannotBeRenamedIfExistingLabelExists(
            "NewGlobalName",
            "OldGlobalName",
            null,
            labelCounter);

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenRenamingGlobalLabelAndNewNameDoesNotExist_ShouldReturnFalse()
    {
        var labelCounter = new TestLabelCounter(0);
        var rule = new LabelCannotBeRenamedIfExistingLabelExists(
            "NewGlobalName",
            "OldGlobalName",
            null,
            labelCounter);

        rule.IsBroken().ShouldBeFalse();
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
