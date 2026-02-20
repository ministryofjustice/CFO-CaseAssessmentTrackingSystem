#nullable enable
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.Commands;

public class AddLabelCommandTests
{
    private TestLabelRepository _repository = null!;
    private TestLabelCounter _labelCounter = null!;
    private AddLabel.Handler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new TestLabelRepository();
        _labelCounter = new TestLabelCounter();
        _handler = new AddLabel.Handler(_repository, _labelCounter);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldCreateLabel()
    {
        var command = new AddLabel.Command
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
        _repository.AddedLabel.ShouldNotBeNull();
        _repository.AddedLabel.Name.ShouldBe("Test Label");
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldCallRepositoryAdd()
    {
        var command = new AddLabel.Command
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

        _repository.AddAsyncCalled.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithInvalidName_ShouldFail()
    {
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        var validator = new AddLabel.Validator();
        var command = new AddLabel.Command
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
        _labelCounter.SetVisibleLabelCount(1);
        var command = new AddLabel.Command
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
        var command = new AddLabel.Command
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

        _repository.AddedLabel.ShouldNotBeNull();
        _repository.AddedLabel.ContractId.ShouldBeNull();
    }

    [Test]
    public async Task Handle_WithContractScope_ShouldCreateWithContractId()
    {
        var command = new AddLabel.Command
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

        _repository.AddedLabel.ShouldNotBeNull();
        _repository.AddedLabel.ContractId.ShouldBe("CONTRACT-001");
    }

    private class TestLabelRepository : ILabelRepository
    {
        public Label? AddedLabel { get; private set; }
        public bool AddAsyncCalled { get; private set; }

        public Task AddAsync(Label label)
        {
            AddedLabel = label;
            AddAsyncCalled = true;
            return Task.CompletedTask;
        }

        public Task<Label?> GetByIdAsync(LabelId labelId) => Task.FromResult<Label?>(null);
        public Task<int> CountParticipants(LabelId labelId) => Task.FromResult(0);
    }

    private class TestLabelCounter : ILabelCounter
    {
        private int _visibleLabelCount;

        public void SetVisibleLabelCount(int count) => _visibleLabelCount = count;

        public int CountVisibleLabels(string name, string? contractId) => _visibleLabelCount;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
