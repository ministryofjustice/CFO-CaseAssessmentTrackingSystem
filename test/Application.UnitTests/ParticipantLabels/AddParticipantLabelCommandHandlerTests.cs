#nullable enable
using Cfo.Cats.Application.Features.ParticipantLabels.AddParticipantLabel;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.ParticipantLabels;

public class AddParticipantLabelCommandHandlerTests
{
    private Mock<ILabelRepository> _labelRepository = null!;
    private Mock<IParticipantLabelRepository> _participantLabelRepository = null!;
    private Mock<IParticipantLabelsCounter> _counter = null!;
    private AddParticipantLabelCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _labelRepository = new Mock<ILabelRepository>();
        _participantLabelRepository = new Mock<IParticipantLabelRepository>();
        _counter = new Mock<IParticipantLabelsCounter>();
        _handler = new AddParticipantLabelCommandHandler(_labelRepository.Object, _participantLabelRepository.Object, _counter.Object);
    }

    [Test]
    public async Task Handle_WithValidLabelId_ShouldSucceed()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel();
        var command = new AddParticipantLabelCommand(participantId, label.Id);

        _labelRepository.Setup(r => r.GetByIdAsync(label.Id))
            .ReturnsAsync(label);
        _counter.Setup(c => c.CountOpenLabels(participantId, label.Id))
            .Returns(0);

        ParticipantLabel? addedParticipantLabel = null;
        _participantLabelRepository.Setup(r => r.AddAsync(It.IsAny<ParticipantLabel>()))
            .Callback<ParticipantLabel>(pl => addedParticipantLabel = pl)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        addedParticipantLabel.ShouldNotBeNull();
    }

    [Test]
    public async Task Handle_WithInvalidLabelId_ShouldReturnFailure()
    {
        var participantId = new ParticipantId("PART00001");
        var labelId = new LabelId(Guid.NewGuid());
        var command = new AddParticipantLabelCommand(participantId, labelId);

        _labelRepository.Setup(r => r.GetByIdAsync(labelId))
            .ReturnsAsync((Label)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain("Invalid label id");
        _participantLabelRepository.Verify(r => r.AddAsync(It.IsAny<ParticipantLabel>()), Times.Never);
    }

    [Test]
    public async Task Handle_ShouldAddParticipantLabelToRepository()
    {
        var participantId = new ParticipantId("PART00001");
        var label = CreateTestLabel();
        var command = new AddParticipantLabelCommand(participantId, label.Id);

        _labelRepository.Setup(r => r.GetByIdAsync(label.Id))
            .ReturnsAsync(label);
        _counter.Setup(c => c.CountOpenLabels(participantId, label.Id))
            .Returns(0);

        ParticipantLabel? addedParticipantLabel = null;
        _participantLabelRepository.Setup(r => r.AddAsync(It.IsAny<ParticipantLabel>()))
            .Callback<ParticipantLabel>(pl => addedParticipantLabel = pl)
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        _participantLabelRepository.Verify(r => r.AddAsync(It.IsAny<ParticipantLabel>()), Times.Once);
        addedParticipantLabel.ShouldNotBeNull();
        addedParticipantLabel.ParticipantId.Value.ShouldBe(participantId.Value);
        addedParticipantLabel.Label.Id.Value.ShouldBe(label.Id.Value);
    }

    private Label CreateTestLabel()
    {
        var mockLabelCounter = new Mock<ILabelCounter>();
        return Label.Create(
            "Test Label",
            "Test Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            mockLabelCounter.Object);
    }
}
