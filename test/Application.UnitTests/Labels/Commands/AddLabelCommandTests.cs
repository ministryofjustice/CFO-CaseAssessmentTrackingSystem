#nullable enable
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.Commands;

public class AddLabelCommandTests
{
    private Mock<ILabelRepository> _repository = null!;
    private Mock<ILabelCounter> _labelCounter = null!;
    private AddLabelCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<ILabelRepository>();
        _labelCounter = new Mock<ILabelCounter>();
        _handler = new AddLabelCommandHandler(_repository.Object, _labelCounter.Object);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldCreateLabel()
    {
        Label? addedLabel = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<Label>()))
            .Callback<Label>(label => addedLabel = label)
            .Returns(Task.CompletedTask);

        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Test Label",
            Description = "Test Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        addedLabel.ShouldNotBeNull();
        addedLabel.Name.ShouldBe("Test Label");
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldCallRepositoryAdd()
    {
        _repository.Setup(r => r.AddAsync(It.IsAny<Label>()))
            .Returns(Task.CompletedTask);

        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "New Label",
            Description = "Description",
            Colour = AppColour.Secondary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Person,
            ContractId = null
        };

        await _handler.Handle(command, CancellationToken.None);

        _repository.Verify(r => r.AddAsync(It.IsAny<Label>()), Times.Once);
    }

    [Test]
    public void Validator_WithInvalidName_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "",
            Description = "Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
    }

    [Test]
    public void Validator_WithNameTooShort_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "A",
            Description = "Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
    }

    [Test]
    public void Validator_WithNameTooLong_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = new  string('A', 201),
            Description = "Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
    }

    [Test]
    public void Validator_WithDescriptionOver200Chars_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = new string('A', 201),
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
    }

    [Test]
    public void Validator_WithDescriptionTooShort_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = "AB",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
    }

    [Test]
    public void Validator_WithValidCommand_ShouldPass()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = "Valid Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNameContainingInvalidCharacters_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Invalid123",
            Description = "Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name" && 
            e.ErrorMessage.Contains("must contain only letters, spaces, and underscores"));
    }

    [Test]
    public void Validator_WithNameContainingUnderscores_ShouldPass()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid_Name",
            Description = "Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithDescriptionContainingCommonPunctuation_ShouldPass()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = "Description with £, $, @, punctuation & symbols!",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithDescriptionContainingNumbers_ShouldPass()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = "Description 123 with numbers",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithDescriptionContainingMultiLine_ShouldPass()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = "Line 1\r\nLine 2",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithDescriptionContainingInvalidCharacters_ShouldFail()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Valid",
            Description = "Invalid characters: <html> tags",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description" && 
            e.ErrorMessage.Contains("must contain only letters, numbers, spaces and common punctuation"));
    }

    [Test]
    public void Validator_WithNameAt25Characters_ShouldPass()
    {
        var validator = new AddLabelCommandValidator();
        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = new string('A', 25),
            Description = "Valid Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Handle_WithDuplicateName_ShouldThrowBusinessRuleException()
    {
        _labelCounter.Setup(c => c.CountVisibleLabels(It.IsAny<string>(), It.IsAny<string?>()))
            .Returns(1);

        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Duplicate",
            Description = "Description",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WithGlobalScope_ShouldCreateWithNullContractId()
    {
        Label? addedLabel = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<Label>()))
            .Callback<Label>(label => addedLabel = label)
            .Returns(Task.CompletedTask);

        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Global",
            Description = "Global Label",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = null
        };

        await _handler.Handle(command, CancellationToken.None);

        addedLabel.ShouldNotBeNull();
        addedLabel.ContractId.ShouldBeNull();
    }

    [Test]
    public async Task Handle_WithContractScope_ShouldCreateWithContractId()
    {
        Label? addedLabel = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<Label>()))
            .Callback<Label>(label => addedLabel = label)
            .Returns(Task.CompletedTask);

        var command = new AddLabelCommand
        {
            Scope = LabelScope.User,
            Name = "Contract",
            Description = "Contract Label",
            Colour = AppColour.Primary,
            Variant = AppVariant.Filled,
            AppIcon = AppIcon.Label,
            ContractId = "CONTRACT-001"
        };

        await _handler.Handle(command, CancellationToken.None);

        addedLabel.ShouldNotBeNull();
        addedLabel.ContractId.ShouldBe("CONTRACT-001");
    }
}
