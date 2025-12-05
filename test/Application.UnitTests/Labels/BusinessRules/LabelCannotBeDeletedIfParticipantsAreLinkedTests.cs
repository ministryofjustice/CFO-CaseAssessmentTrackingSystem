#nullable enable
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Labels.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.BusinessRules;

public class LabelCannotBeDeletedIfParticipantsAreLinkedTests
{
    [Test]
    public void IsBroken_WhenParticipantsAreLinked_ShouldReturnTrue()
    {
        var labelId = new LabelId(Guid.NewGuid());
        var labelCounter = new TestLabelCounter(5);
        var rule = new LabelCannotBeDeletedIfParticipantsAreLinked(labelId, labelCounter);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("Label cannot be deleted because there are participants linked to it.");
    }

    [Test]
    public void IsBroken_WhenNoParticipantsLinked_ShouldReturnFalse()
    {
        var labelId = new LabelId(Guid.NewGuid());
        var labelCounter = new TestLabelCounter(0);
        var rule = new LabelCannotBeDeletedIfParticipantsAreLinked(labelId, labelCounter);

        rule.IsBroken().ShouldBeFalse();
    }

    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    public void IsBroken_WhenAnyParticipantsLinked_ShouldReturnTrue(int count)
    {
        var labelId = new LabelId(Guid.NewGuid());
        var labelCounter = new TestLabelCounter(count);
        var rule = new LabelCannotBeDeletedIfParticipantsAreLinked(labelId, labelCounter);

        rule.IsBroken().ShouldBeTrue();
    }

    private class TestLabelCounter : ILabelCounter
    {
        private readonly int _participantCount;

        public TestLabelCounter(int participantCount)
        {
            _participantCount = participantCount;
        }

        public int CountVisibleLabels(string name, string? contractId) => 0;
        public int CountParticipants(LabelId labelId) => _participantCount;
    }
}
