#nullable enable
using System.Linq;
using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Labels.Events;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Labels;

public class LabelTests
{
    private static readonly string[] OneContract = ["CONTRACT-001"];

    private TestLabelCounter _labelCounter = null!;

    [SetUp]
    public void Setup() => _labelCounter = new TestLabelCounter();

    [Test]
    public void Create_WithValidData_ShouldSucceed()
    {
        var label = Label.Create(
            "Test Label",
            "Test Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

        label.ShouldNotBeNull();
        label.Name.ShouldBe("Test Label");
        label.Description.ShouldBe("Test Description");
        label.Scope.ShouldBe(LabelScope.User);
        label.Colour.ShouldBe(AppColour.Primary);
        label.Variant.ShouldBe(AppVariant.Filled);
        label.Contracts.Select(c => c.ContractId).ShouldContain("CONTRACT-001");
    }

    [Test]
    public void Create_WithMultipleContracts_ShouldAssignAllDistinctContracts()
    {
        var label = Label.Create(
            "Multi",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            ["CONTRACT-001", "CONTRACT-002", "CONTRACT-001"],
            _labelCounter);

        label.Contracts.Select(c => c.ContractId).ShouldBe(["CONTRACT-001", "CONTRACT-002"], ignoreOrder: true);
    }

    [Test]
    public void Create_WithNullName_ShouldThrowBusinessRuleException() => Should.Throw<BusinessRuleValidationException>(() =>
                                                                                   Label.Create(
                                                                                       null!,
                                                                                       "Description",
                                                                                       LabelScope.User,
                                                                                       AppColour.Primary,
                                                                                       AppVariant.Filled,
                                                                                       AppIcon.Label,
                                                                                       OneContract,
                                                                                       _labelCounter))
            .Message.ShouldContain("Label Name cannot be null or empty.");

    [Test]
    public void Create_WithEmptyName_ShouldThrowBusinessRuleException() => Should.Throw<BusinessRuleValidationException>(() =>
                                                                                    Label.Create(
                                                                                        "",
                                                                                        "Description",
                                                                                        LabelScope.User,
                                                                                        AppColour.Primary,
                                                                                        AppVariant.Filled,
                                                                                        AppIcon.Label,
                                                                                        OneContract,
                                                                                        _labelCounter))
            .Message.ShouldContain("Label Name cannot be null or empty.");

    [Test]
    public void Create_WithNameTooShort_ShouldThrowBusinessRuleException() => Should.Throw<BusinessRuleValidationException>(() =>
                                                                                       Label.Create(
                                                                                           "A",
                                                                                           "Description",
                                                                                           LabelScope.User,
                                                                                           AppColour.Primary,
                                                                                           AppVariant.Filled,
                                                                                           AppIcon.Label,
                                                                                           OneContract,
                                                                                           _labelCounter))
            .Message.ShouldContain("Label must be between 2 and 40 characters");

    [Test]
    public void Create_WithDuplicateName_ShouldThrowBusinessRuleException()
    {
        _labelCounter.SetVisibleLabelCount(1);

        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                "Duplicate",
                "Description",
                LabelScope.User,
                AppColour.Primary,
                AppVariant.Filled,
                AppIcon.Label,
                OneContract,
                _labelCounter))
            .Message.ShouldContain("Label names must be unique");
    }

    [Test]
    public void Create_ShouldRaiseLabelCreatedDomainEvent()
    {
        var label = Label.Create(
            "Test Label",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelCreatedDomainEvent);
    }

    [Test]
    public void Edit_WithValidChanges_ShouldUpdateAllProperties()
    {
        var label = CreateLabel("Original", "Original Description");

        label.Edit(
            "Updated",
            "Updated Description",
            LabelScope.User,
            AppColour.Secondary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

        label.Name.ShouldBe("Updated");
        label.Description.ShouldBe("Updated Description");
        label.Colour.ShouldBe(AppColour.Secondary);
        label.Variant.ShouldBe(AppVariant.Filled);
    }

    [Test]
    public void Edit_WhenContractsChange_ShouldReplaceContracts()
    {
        var label = CreateLabel("Original", "Description");

        label.Edit(
            "Original",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            ["CONTRACT-002", "CONTRACT-003"],
            _labelCounter);

        label.Contracts.Select(c => c.ContractId).ShouldBe(["CONTRACT-002", "CONTRACT-003"], ignoreOrder: true);
    }

    [Test]
    public void Edit_WhenNameChanges_ShouldRaiseLabelRenamedEvent()
    {
        var label = CreateLabel("Original", "Description");
        label.ClearDomainEvents();

        label.Edit(
            "NewName",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelRenamedDomainEvent);
    }

    [Test]
    public void Edit_WhenColourChanges_ShouldRaiseLabelColourChangedEvent()
    {
        var label = CreateLabel("Label", "Description");
        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Description",
            LabelScope.User,
            AppColour.Secondary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelColourChangedDomainEvent);
    }

    [Test]
    public void Edit_WhenScopeChanges_ShouldRaiseLabelScopeChangedEvent()
    {
        var label = CreateLabel("Label", "Description");
        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Description",
            LabelScope.System,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelScopeChangedDomainEvent);
        label.Scope.ShouldBe(LabelScope.System);
    }

    [Test]
    public void Edit_WhenAppIconChanges_ShouldRaiseLabelAppIconChangedEvent()
    {
        var label = CreateLabel("Label", "Description");
        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Description",
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Star,
            OneContract,
            _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelAppIconChangedDomainEvent);
        label.AppIcon.ShouldBe(AppIcon.Star);
    }

    [Test]
    public void Delete_WithoutLinkedParticipants_ShouldSucceed()
    {
        var label = CreateLabel("Label", "Description");
        _labelCounter.SetParticipantCount(0);

        label.Delete(_labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelDeletedDomainEvent);
    }

    [Test]
    public void Delete_WithLinkedParticipants_ShouldThrowBusinessRuleException()
    {
        var label = CreateLabel("Label", "Description");
        _labelCounter.SetParticipantCount(5);

        Should.Throw<BusinessRuleValidationException>(() =>
            label.Delete(_labelCounter))
            .Message.ShouldContain("Label cannot be deleted because there are participants linked to it");
    }

    [Test]
    public void Delete_ShouldRaiseLabelDeletedEvent()
    {
        var label = CreateLabel("Label", "Description");
        _labelCounter.SetParticipantCount(0);

        label.Delete(_labelCounter);

        var deleteEvent = label.DomainEvents.OfType<LabelDeletedDomainEvent>().FirstOrDefault();
        deleteEvent.ShouldNotBeNull();
        deleteEvent.Entity.ShouldBe(label);
    }

    [Test]
    public void Edit_WhenRenamingToExistingLabel_ShouldThrowBusinessRuleException()
    {
        var label = CreateLabel("Original", "Description");
        _labelCounter.SetVisibleLabelCount(1);

        Should.Throw<BusinessRuleValidationException>(() =>
            label.Edit(
                "ExistingLabel",
                "Description",
                LabelScope.User,
                AppColour.Primary,
                AppVariant.Filled,
                AppIcon.Label,
                OneContract,
                _labelCounter))
            .Message.ShouldContain("Cannot rename label");
    }

    [Test]
    public void Edit_WhenKeepingSameName_ShouldNotCheckForDuplicates()
    {
        var label = CreateLabel("Original", "Description");
        _labelCounter.SetVisibleLabelCount(1);

        label.Edit(
            "Original",
            "Updated Description",
            LabelScope.User,
            AppColour.Secondary,
            AppVariant.Filled,
            AppIcon.Star,
            OneContract,
            _labelCounter);

        label.Name.ShouldBe("Original");
        label.Description.ShouldBe("Updated Description");
        label.Colour.ShouldBe(AppColour.Secondary);
    }

    private Label CreateLabel(string name, string description) =>
        Label.Create(
            name,
            description,
            LabelScope.User,
            AppColour.Primary,
            AppVariant.Filled,
            AppIcon.Label,
            OneContract,
            _labelCounter);

    private class TestLabelCounter : ILabelCounter
    {
        private int _visibleLabelCount;
        private int _participantCount;

        public void SetVisibleLabelCount(int count) => _visibleLabelCount = count;
        public void SetParticipantCount(int count) => _participantCount = count;

        public int CountLabelsWithName(string name) => _visibleLabelCount;
        public int CountParticipants(LabelId labelId) => _participantCount;
    }
}
