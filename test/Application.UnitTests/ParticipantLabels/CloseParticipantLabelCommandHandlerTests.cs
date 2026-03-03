#nullable enable
using Cfo.Cats.Application.Features.ParticipantLabels.CloseLabel;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.ParticipantLabels;

public class CloseParticipantLabelCommandHandlerTests
{
    private TestParticipantLabelRepository _repository = null!;
    private TestLabelCounter _labelCounter = null!;
    private TestParticipantLabelsCounter _participantLabelsCounter = null!;
    private CloseParticipantLabelCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new TestParticipantLabelRepository();
        _labelCounter = new TestLabelCounter();
        _participantLabelsCounter = new TestParticipantLabelsCounter();
        _handler = new CloseParticipantLabelCommandHandler(_repository);
    }

    [Test]
    public async Task Handle_WithValidParticipantLabelId_ShouldSucceed()
    {
        var participantLabel = CreateTestParticipantLabel(LabelScope.User);
        var command = new CloseParticipantLabelCommand(participantLabel.Id);

        _repository.SetParticipantLabel(participantLabel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        participantLabel.Lifetime.EndDate.ShouldNotBe(DateTime.MaxValue);
    }

    [Test]
    public async Task Handle_ShouldCloseTheLabel()
    {
        var participantLabel = CreateTestParticipantLabel(LabelScope.User);
        var command = new CloseParticipantLabelCommand(participantLabel.Id);
        var originalEndDate = participantLabel.Lifetime.EndDate;

        _repository.SetParticipantLabel(participantLabel);

        await _handler.Handle(command, CancellationToken.None);

        participantLabel.Lifetime.EndDate.ShouldNotBe(originalEndDate);
        participantLabel.Lifetime.EndDate.ShouldNotBe(DateTime.MaxValue);
        _repository.GetByIdAsyncCalled.ShouldBeTrue();
    }

    private ParticipantLabel CreateTestParticipantLabel(LabelScope scope)
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel(scope);

        return ParticipantLabel.Create(participantId, label, _participantLabelsCounter);
    }

    private Label CreateTestLabel(LabelScope scope)
     => Label.Create(
            "Test Label",
            "Test Description",
            scope,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            _labelCounter);
    

    private class TestParticipantLabelRepository : IParticipantLabelRepository
    {
        private ParticipantLabel? _participantLabel;
        public bool GetByIdAsyncCalled { get; private set; }

        public void SetParticipantLabel(ParticipantLabel? participantLabel) => _participantLabel = participantLabel;

        public Task<ParticipantLabel> GetByIdAsync(ParticipantLabelId participantLabelId)
        {
            GetByIdAsyncCalled = true;
            return Task.FromResult(_participantLabel!);
        }

        public Task AddAsync(ParticipantLabel participantLabel) => Task.CompletedTask;
        public Task<ParticipantLabel[]> GetByParticipantIdAsync(ParticipantId participantId) => throw new NotImplementedException();

    }

    private class TestParticipantLabelsCounter : IParticipantLabelsCounter
    {
        public int CountOpenLabels(ParticipantId participantId, LabelId labelId) => 0;
    }

    private class TestLabelCounter : ILabelCounter
    {
        public int CountVisibleLabels(string name, string? contractId) => 0;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
