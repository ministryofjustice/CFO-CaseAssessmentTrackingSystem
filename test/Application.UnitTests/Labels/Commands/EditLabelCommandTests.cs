#nullable enable
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.Commands;

public class EditLabelCommandTests
{
    private TestLabelRepository _repository = null!;
    private TestLabelCounter _labelCounter = null!;
    private EditLabel.Handler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new TestLabelRepository();
        _labelCounter = new TestLabelCounter();
        _handler = new EditLabel.Handler(_repository, _labelCounter);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldUpdateLabel()
    {
        var existingLabel = Label.Create(
            "Original",
            "Original Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Updated",
            NewDescription = "Updated Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Secondary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Person
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        existingLabel.Name.ShouldBe("Updated");
        existingLabel.Description.ShouldBe("Updated Description");
        existingLabel.Colour.ShouldBe(AppColour.Secondary);
        existingLabel.Variant.ShouldBe(AppVariant.Filled);
    }

    [Test]
    public void Handle_WithNonExistentLabel_ShouldThrowNotFoundException()
    {
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Updated",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        Should.Throw<NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None))
            .Message.ShouldContain("Label does not exist");
    }

    [Test]
    public async Task Handle_ShouldUpdateAllModifiedProperties()
    {
        var existingLabel = Label.Create(
            "Name1",
            "Desc1",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Name2",
            NewDescription = "Desc2",
            NewScope = LabelScope.System,
            NewColour = AppColour.Info,
            NewVariant = AppVariant.Text,
            NewAppIcon = AppIcon.Star
        };

        await _handler.Handle(command, CancellationToken.None);

        existingLabel.Name.ShouldBe("Name2");
        existingLabel.Description.ShouldBe("Desc2");
        existingLabel.Scope.ShouldBe(LabelScope.System);
        existingLabel.Colour.ShouldBe(AppColour.Info);
        existingLabel.Variant.ShouldBe(AppVariant.Text);
    }

    [Test]
    public async Task Handle_WhenOnlyNameChanges_ShouldUpdateOnlyName()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Changed",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        await _handler.Handle(command, CancellationToken.None);

        existingLabel.Name.ShouldBe("Changed");
        existingLabel.Description.ShouldBe("Description");
    }

    [Test]
    public void Validator_WithInvalidNewName_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewName");
    }

    [Test]
    public void Validator_WithNewNameTooShort_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "A",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewName");
    }

    [Test]
    public void Validator_WithNewNameTooLong_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = new string('x', 201),
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewName");
    }

    [Test]
    public void Validator_WithNewDescriptionEmpty_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewDescription");
    }

    [Test]
    public void Validator_WithNewDescriptionTooShort_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "AB",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewDescription");
    }

    [Test]
    public void Validator_WithNewDescriptionTooLong_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = new string('A', 201),
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewDescription");
    }

    [Test]
    public void Validator_WithValidCommand_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Valid Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNewNameContainingInvalidCharacters_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Invalid123",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewName" && 
            e.ErrorMessage.Contains("must contain only letters, spaces, and underscores"));
    }
    
    [Test]
    public void Validator_WithNewNameContainingUnderscores_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid_Name",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNewDescriptionContainingCommonPunctuation_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Description with £, $, @, punctuation & symbols!",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNewDescriptionContainingNumbers_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Description 123 with numbers",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNewDescriptionContainingMultiLine_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Line 1\r\nLine 2",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNewDescriptionContainingInvalidCharacters_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Invalid characters: <html> tags",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewDescription" && 
            e.ErrorMessage.Contains("must contain only letters, numbers, spaces and common punctuation"));
    }

    [Test]
    public void Validator_WithNewNameAt25Characters_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = new string('A', 25),
            NewDescription = "Valid Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Handle_WhenRenamingToExistingLabel_ShouldThrowBusinessRuleException()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);
        _labelCounter.SetVisibleLabelCount(1);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "ExistingLabel",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(command, CancellationToken.None))
            .Message.ShouldContain("Cannot rename label");
    }

    [Test]
    public async Task Handle_WhenRenamingToNonExistingLabel_ShouldSucceed()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);
        _labelCounter.SetVisibleLabelCount(0);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "NewUniqueName",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        existingLabel.Name.ShouldBe("NewUniqueName");
    }

    [Test]
    public async Task Handle_WhenScopeChanges_ShouldUpdateScope()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Original",
            NewDescription = "Description",
            NewScope = LabelScope.System,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        existingLabel.Scope.ShouldBe(LabelScope.System);
    }

    [Test]
    public async Task Handle_WhenScopeRemainsTheSame_ShouldNotChangeScope()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Original",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Label
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        existingLabel.Scope.ShouldBe(LabelScope.User);
    }

    [Test]
    public async Task Handle_WhenAppIconChanges_ShouldUpdateAppIcon()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Original",
            NewDescription = "Description",
            NewScope = LabelScope.User,
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled,
            NewAppIcon = AppIcon.Star
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        existingLabel.AppIcon.ShouldBe(AppIcon.Star);
    }
    
    private class TestLabelRepository : ILabelRepository
    {
        private Label? _existingLabel;

        public void SetExistingLabel(Label label) => _existingLabel = label;

        public Task AddAsync(Label label) => Task.CompletedTask;

        public Task<Label?> GetByIdAsync(LabelId labelId)
        {
            return Task.FromResult(_existingLabel != null && _existingLabel.Id == labelId ? _existingLabel : null);
        }

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
