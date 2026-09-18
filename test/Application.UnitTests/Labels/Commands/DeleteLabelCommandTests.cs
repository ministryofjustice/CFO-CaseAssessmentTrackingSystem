#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Application.Features.Labels.Commands.DeleteLabel;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.Commands;

public class DeleteLabelCommandTests
{
    private Mock<ILabelRepository> _repository = null!;
    private Mock<ILabelCounter> _labelCounter = null!;
    private DeleteLabelCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<ILabelRepository>();
        _labelCounter = new Mock<ILabelCounter>();
        _handler = new DeleteLabelCommandHandler(_repository.Object, _labelCounter.Object);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldDeleteLabel()
    {
        var label = CreateLabel("ToDelete");

        _repository.Setup(r => r.GetByIdAsync(label.Id))
            .ReturnsAsync(label);
        _labelCounter.Setup(c => c.CountParticipants(label.Id))
            .Returns(0);

        var command = new DeleteLabelCommand
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = label.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    [Test]
    public void Handle_WithLinkedParticipants_ShouldThrowBusinessRuleException()
    {
        var label = CreateLabel("LinkedLabel");

        _repository.Setup(r => r.GetByIdAsync(label.Id))
            .ReturnsAsync(label);
        _labelCounter.Setup(c => c.CountParticipants(label.Id))
            .Returns(5);

        var command = new DeleteLabelCommand
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = label.Id
        };

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None))
            .Message.ShouldContain("Label cannot be deleted because there are participants linked to it");
    }

    private static Label CreateLabel(string name) => Label.Create(
        name,
        "Description",
        LabelScope.User,
        AppColour.Primary,
        AppVariant.Filled,
        AppIcon.Label,
        ["CONTRACT-001"],
        new Mock<ILabelCounter>().Object);

    private static UserProfile CreateInternalUserProfile() => new UserProfile
    {
        UserId = Guid.NewGuid().ToString(),
        UserName = "internal.user",
        Email = "internal.user@justice.gov.uk",
        TenantId = "1."
    };
}
