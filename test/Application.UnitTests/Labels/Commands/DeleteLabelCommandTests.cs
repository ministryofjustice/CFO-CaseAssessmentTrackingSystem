#nullable enable
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.Commands;
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
        var mockLabelCounter = new Mock<ILabelCounter>();
        var label = Label.Create(
            "ToDelete",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            mockLabelCounter.Object);

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
    public void Handle_AsExternalUserOnGlobalLabel_ShouldThrowBusinessRuleException()
    {
        var mockLabelCounter = new Mock<ILabelCounter>();
        var globalLabel = Label.Create(
            "Global",
            "Description",
            LabelScope.System,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            null,
            mockLabelCounter.Object);

        _repository.Setup(r => r.GetByIdAsync(globalLabel.Id))
            .ReturnsAsync(globalLabel);
        _labelCounter.Setup(c => c.CountParticipants(globalLabel.Id))
            .Returns(0);

        var command = new DeleteLabelCommand
        {
            UserProfile = CreateExternalUserProfile(),
            LabelId = globalLabel.Id
        };

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None))
            .Message.ShouldContain("You do not have permission to perform this action");
    }

    [Test]
    public async Task Handle_AsInternalUserOnGlobalLabel_ShouldSucceed()
    {
        var mockLabelCounter = new Mock<ILabelCounter>();
        var globalLabel = Label.Create(
            "Global",
            "Description",
            LabelScope.System,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            null,
            mockLabelCounter.Object);

        _repository.Setup(r => r.GetByIdAsync(globalLabel.Id))
            .ReturnsAsync(globalLabel);
        _labelCounter.Setup(c => c.CountParticipants(globalLabel.Id))
            .Returns(0);

        var command = new DeleteLabelCommand
        {
            UserProfile = CreateInternalUserProfile(),
            LabelId = globalLabel.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    [Test]
    public void Handle_WithLinkedParticipants_ShouldThrowBusinessRuleException()
    {
        var mockLabelCounter = new Mock<ILabelCounter>();
        var label = Label.Create(
            "LinkedLabel",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            mockLabelCounter.Object);

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

    [Test]
    public async Task Handle_AsExternalUserOnContractLabel_ShouldSucceed()
    {
        var mockLabelCounter = new Mock<ILabelCounter>();
        var contractLabel = Label.Create(
            "Contract",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            mockLabelCounter.Object);

        _repository.Setup(r => r.GetByIdAsync(contractLabel.Id))
            .ReturnsAsync(contractLabel);
        _labelCounter.Setup(c => c.CountParticipants(contractLabel.Id))
            .Returns(0);

        var command = new DeleteLabelCommand
        {
            UserProfile = CreateExternalUserProfile(),
            LabelId = contractLabel.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    private static UserProfile CreateInternalUserProfile() => new UserProfile
    {
        UserId = Guid.NewGuid().ToString(),
        UserName = "internal.user",
        Email = "internal.user@justice.gov.uk",
        TenantId = "1."
    };

    private static UserProfile CreateExternalUserProfile() => new UserProfile
    {
        UserId = Guid.NewGuid().ToString(),
        UserName = "external.user",
        Email = "external.user@example.com",
        TenantId = "1."
    };
}
