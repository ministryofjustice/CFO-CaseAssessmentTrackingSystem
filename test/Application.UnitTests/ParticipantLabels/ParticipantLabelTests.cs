#nullable enable
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.ParticipantLabels.Events;
using Cfo.Cats.Domain.Participants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.ParticipantLabels;

public class ParticipantLabelTests
{
    private TestParticipantLabelsCounter _counter = null!;
    private TestLabelCounter _labelCounter = null!;

    [SetUp]
    public void Setup()
    {
        _counter = new TestParticipantLabelsCounter();
        _labelCounter = new TestLabelCounter();
    }

    [Test]
    public void Create_WithValidData_ShouldSucceed()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel();

        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);

        participantLabel.ShouldNotBeNull();
        participantLabel.ParticipantId.Value.ShouldBe("PART00001");
        participantLabel.Label.ShouldBe(label);
        participantLabel.Lifetime.StartDate.ShouldNotBe(default);
        participantLabel.Lifetime.EndDate.ShouldBe(DateTime.MaxValue);
    }

    [Test]
    public void Create_ShouldRaiseParticipantLabelCreatedDomainEvent()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel();

        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);

        participantLabel.DomainEvents.ShouldContain(e => e is ParticipantLabelCreatedDomainEvent);
    }

    [Test]
    public void Create_WhenLabelAlreadyOpen_ShouldThrowBusinessRuleException()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel();
        _counter.SetOpenLabelCount(1);

        Should.Throw<BusinessRuleValidationException>(() =>
            ParticipantLabel.Create(participantId, label, _counter))
            .Message.ShouldContain("Participant labels must be unique");
    }

    [Test]
    public void Create_WhenLabelNotAlreadyOpen_ShouldSucceed()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel();
        _counter.SetOpenLabelCount(0);

        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);

        participantLabel.ShouldNotBeNull();
    }

    [Test]
    public void Close_OnUserScopedLabel_ShouldSucceed()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel(LabelScope.User);
        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);
        
        participantLabel.ClearDomainEvents();

        participantLabel.Close();

        participantLabel.Lifetime.EndDate.ShouldNotBe(DateTime.MaxValue);
        participantLabel.DomainEvents.ShouldContain(e => e is ParticipantLabelClosedDomainEvent);
    }

    [Test]
    public void Close_OnSystemScopedLabelWithoutForce_ShouldThrowBusinessRuleException()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel(LabelScope.System);
        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);

        Should.Throw<BusinessRuleValidationException>(() =>
            participantLabel.Close(false))
            .Message.ShouldContain("System labels cannot be manually closed");
    }

    [Test]
    public void Close_OnSystemScopedLabelWithForce_ShouldSucceed()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel(LabelScope.System);
        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);
        
        participantLabel.ClearDomainEvents();

        participantLabel.Close(force: true);

        participantLabel.Lifetime.EndDate.ShouldNotBe(DateTime.MaxValue);
        participantLabel.DomainEvents.ShouldContain(e => e is ParticipantLabelClosedDomainEvent);
    }

    [Test]
    public void Close_ShouldRaiseParticipantLabelClosedDomainEvent()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel(LabelScope.User);
        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);

        participantLabel.ClearDomainEvents();
        participantLabel.Close();

        var closeEvent = participantLabel.DomainEvents.OfType<ParticipantLabelClosedDomainEvent>().FirstOrDefault();
        closeEvent.ShouldNotBeNull();
        closeEvent.Item.ShouldBe(participantLabel);
    }

    [Test]
    public void ParticipantId_ShouldReturnCorrectValue()
    {
        var participantId = new ParticipantId("PART00123");
        var label = CreateTestLabel();
        var participantLabel = ParticipantLabel.Create(participantId, label, _counter);

        participantLabel.ParticipantId.Value.ShouldBe("PART00123");
    }

    private Label CreateTestLabel(LabelScope? scope = null)
    {
        return Label.Create(
            "Test Label",
            "Test Description",
            scope ?? LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            _labelCounter);
    }

    private class TestParticipantLabelsCounter : IParticipantLabelsCounter
    {
        private int _openLabelCount;

        public void SetOpenLabelCount(int count) => _openLabelCount = count;

        public int CountOpenLabels(ParticipantId participantId, LabelId labelId) => _openLabelCount;
    }

    private class TestLabelCounter : ILabelCounter
    {
        public int CountVisibleLabels(string name, string? contractId) => 0;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
