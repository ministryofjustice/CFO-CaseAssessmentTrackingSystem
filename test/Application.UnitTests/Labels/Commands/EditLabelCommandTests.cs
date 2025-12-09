#nullable enable
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels.Commands;

public class EditLabelCommandTests
{
    private TestLabelRepository _repository = null!;
    private EditLabel.Handler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new TestLabelRepository();
        _handler = new EditLabel.Handler(_repository);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldUpdateLabel()
    {
        var existingLabel = Label.Create(
            "Original",
            "Original Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Updated",
            NewDescription = "Updated Description",
            NewColour = AppColour.Secondary,
            NewVariant = AppVariant.Outlined
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        existingLabel.Name.ShouldBe("Updated");
        existingLabel.Description.ShouldBe("Updated Description");
        existingLabel.Colour.ShouldBe(AppColour.Secondary);
        existingLabel.Variant.ShouldBe(AppVariant.Outlined);
    }

    [Test]
    public void Handle_WithNonExistentLabel_ShouldThrowNotFoundException()
    {
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Updated",
            NewDescription = "Description",
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Name2",
            NewDescription = "Desc2",
            NewColour = AppColour.Info,
            NewVariant = AppVariant.Text
        };

        await _handler.Handle(command, CancellationToken.None);

        existingLabel.Name.ShouldBe("Name2");
        existingLabel.Description.ShouldBe("Desc2");
        existingLabel.Colour.ShouldBe(AppColour.Info);
        existingLabel.Variant.ShouldBe(AppVariant.Text);
    }

    [Test]
    public async Task Handle_WhenOnlyNameChanges_ShouldUpdateOnlyName()
    {
        var existingLabel = Label.Create(
            "Original",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            new TestLabelCounter());

        _repository.SetExistingLabel(existingLabel);

        var command = new EditLabel.Command
        {
            LabelId = existingLabel.Id,
            NewName = "Changed",
            NewDescription = "Description",
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewName = "ThisIsAVeryLongLabelName",
            NewDescription = "Description",
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewName" && 
            e.ErrorMessage.Contains("must contain only letters, spaces, and underscores"));
    }

    [Test]
    public void Validator_WithNewDescriptionContainingInvalidCharacters_ShouldFail()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Invalid@#$",
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewDescription" && 
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
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithNewDescriptionContainingUnderscores_ShouldPass()
    {
        var validator = new EditLabel.Validator();
        var command = new EditLabel.Command
        {
            LabelId = new LabelId(Guid.NewGuid()),
            NewName = "Valid",
            NewDescription = "Valid_Description",
            NewColour = AppColour.Primary,
            NewVariant = AppVariant.Filled
        };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
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
        public int CountVisibleLabels(string name, string? contractId) => 0;
        public int CountParticipants(LabelId labelId) => 0;
    }
}
