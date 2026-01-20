#nullable enable
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
    private TestLabelCounter _labelCounter = null!;

    [SetUp]
    public void Setup()
    {
        _labelCounter = new TestLabelCounter();
    }

    [Test]
    public void Create_WithValidData_ShouldSucceed()
    {
        var label = Label.Create(
            "Test Label",
            "Test Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.ShouldNotBeNull();
        label.Name.ShouldBe("Test Label");
        label.Description.ShouldBe("Test Description");
        label.Colour.ShouldBe(AppColour.Primary);
        label.Variant.ShouldBe(AppVariant.Filled);
        label.ContractId.ShouldBe("CONTRACT-001");
    }

    [Test]
    public void Create_WithNullName_ShouldThrowBusinessRuleException()
    {
        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                null!,
                "Description",
                AppColour.Primary,
                AppVariant.Filled,
                "CONTRACT-001",
                _labelCounter))
            .Message.ShouldContain("Label cannot be null or empty");
    }

    [Test]
    public void Create_WithEmptyName_ShouldThrowBusinessRuleException()
    {
        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                "",
                "Description",
                AppColour.Primary,
                AppVariant.Filled,
                "CONTRACT-001",
                _labelCounter))
            .Message.ShouldContain("Label cannot be null or empty");
    }

    [Test]
    public void Create_WithWhitespaceName_ShouldThrowBusinessRuleException()
    {
        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                "   ",
                "Description",
                AppColour.Primary,
                AppVariant.Filled,
                "CONTRACT-001",
                _labelCounter))
            .Message.ShouldContain("Label cannot be null or empty");
    }

    [Test]
    public void Create_WithNameTooShort_ShouldThrowBusinessRuleException()
    {
        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                "A",
                "Description",
                AppColour.Primary,
                AppVariant.Filled,
                "CONTRACT-001",
                _labelCounter))
            .Message.ShouldContain("Label must be between 2 and 25 characters");
    }

    [Test]
    public void Create_WithNameTooLong_ShouldThrowBusinessRuleException()
    {
        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                new string('x', 201),
                "Description",
                AppColour.Primary,
                AppVariant.Filled,
                "CONTRACT-001",
                _labelCounter))
            .Message.ShouldContain("Label must be between 2 and 25 characters");
    }

    [Test]
    public void Create_WithDuplicateNameSameContract_ShouldThrowBusinessRuleException()
    {
        _labelCounter.SetVisibleLabelCount(1);

        Should.Throw<BusinessRuleValidationException>(() =>
            Label.Create(
                "Duplicate",
                "Description",
                AppColour.Primary,
                AppVariant.Filled,
                "CONTRACT-001",
                _labelCounter))
            .Message.ShouldContain("Labels must be unique at a contract level");
    }

    [Test]
    public void Create_WithDuplicateNameDifferentContract_ShouldSucceed()
    {
        _labelCounter.SetVisibleLabelCount(0);

        var label = Label.Create(
            "Same Name",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-002",
            _labelCounter);

        label.ShouldNotBeNull();
        label.Name.ShouldBe("Same Name");
    }

    [Test]
    public void Create_ShouldRaiseLabelCreatedDomainEvent()
    {
        var label = Label.Create(
            "Test Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelCreatedDomainEvent);
    }

    [Test]
    public void Edit_WithValidChanges_ShouldUpdateAllProperties()
    {
        var label = Label.Create(
            "Original",
            "Original Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.Edit(
            "Updated",
            "Updated Description",
            AppColour.Secondary,
            AppVariant.Outlined);

        label.Name.ShouldBe("Updated");
        label.Description.ShouldBe("Updated Description");
        label.Colour.ShouldBe(AppColour.Secondary);
        label.Variant.ShouldBe(AppVariant.Outlined);
    }

    [Test]
    public void Edit_WhenNameChanges_ShouldRaiseLabelRenamedEvent()
    {
        var label = Label.Create(
            "Original",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.ClearDomainEvents();

        label.Edit(
            "NewName",
            "Description",
            AppColour.Primary,
            AppVariant.Filled);

        label.DomainEvents.ShouldContain(e => e is LabelRenamedDomainEvent);
    }

    [Test]
    public void Edit_WhenColourChanges_ShouldRaiseLabelColourChangedEvent()
    {
        var label = Label.Create(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Description",
            AppColour.Secondary,
            AppVariant.Filled);

        label.DomainEvents.ShouldContain(e => e is LabelColourChangedDomainEvent);
    }

    [Test]
    public void Edit_WhenVariantChanges_ShouldRaiseLabelVariantChangedEvent()
    {
        var label = Label.Create(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Outlined);

        label.DomainEvents.ShouldContain(e => e is LabelVariantChangedDomainEvent);
    }

    [Test]
    public void Edit_WhenDescriptionChanges_ShouldRaiseLabelDescriptionChangedEvent()
    {
        var label = Label.Create(
            "Label",
            "Original",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Updated Description",
            AppColour.Primary,
            AppVariant.Filled);

        label.DomainEvents.ShouldContain(e => e is LabelDescriptionChangedDomainEvent);
    }

    [Test]
    public void Edit_WhenNoChanges_ShouldNotRaiseAnyEvents()
    {
        var label = Label.Create(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        label.ClearDomainEvents();

        label.Edit(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled);

        label.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void Delete_AsInternalUser_OnGlobalLabel_ShouldSucceed()
    {
        var label = Label.Create(
            "Global",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            null,
            _labelCounter);

        var internalUser = new DomainUser("user-1", "internal.user", "1.", true);
        _labelCounter.SetParticipantCount(0);

        label.Delete(internalUser, _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelDeletedDomainEvent);
    }

    [Test]
    public void Delete_AsExternalUser_OnGlobalLabel_ShouldThrowBusinessRuleException()
    {
        var label = Label.Create(
            "Global",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            null,
            _labelCounter);

        var externalUser = new DomainUser("user-1", "external.user", "1.", false);

        Should.Throw<BusinessRuleValidationException>(() =>
            label.Delete(externalUser, _labelCounter))
            .Message.ShouldContain("You do not have permission to perform this action");
    }

    [Test]
    public void Delete_OnContractLabel_ShouldSucceed()
    {
        var label = Label.Create(
            "Contract",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        var externalUser = new DomainUser("user-1", "external.user", "1.", false);
        _labelCounter.SetParticipantCount(0);

        label.Delete(externalUser, _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelDeletedDomainEvent);
    }

    [Test]
    public void Delete_WithLinkedParticipants_ShouldThrowBusinessRuleException()
    {
        var label = Label.Create(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        var user = new DomainUser("user-1", "user", "1.", true);
        _labelCounter.SetParticipantCount(5);

        Should.Throw<BusinessRuleValidationException>(() =>
            label.Delete(user, _labelCounter))
            .Message.ShouldContain("Label cannot be deleted because there are participants linked to it");
    }

    [Test]
    public void Delete_WithoutLinkedParticipants_ShouldSucceed()
    {
        var label = Label.Create(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        var user = new DomainUser("user-1", "user", "1.", true);
        _labelCounter.SetParticipantCount(0);

        label.Delete(user, _labelCounter);

        label.DomainEvents.ShouldContain(e => e is LabelDeletedDomainEvent);
    }

    [Test]
    public void Delete_ShouldRaiseLabelDeletedEvent()
    {
        var label = Label.Create(
            "Label",
            "Description",
            AppColour.Primary,
            AppVariant.Filled,
            "CONTRACT-001",
            _labelCounter);

        var user = new DomainUser("user-1", "user", "1.", true);
        _labelCounter.SetParticipantCount(0);

        label.Delete(user, _labelCounter);

        var deleteEvent = label.DomainEvents.OfType<LabelDeletedDomainEvent>().FirstOrDefault();
        deleteEvent.ShouldNotBeNull();
        deleteEvent.Entity.ShouldBe(label);
    }

    private class TestLabelCounter : ILabelCounter
    {
        private int _visibleLabelCount;
        private int _participantCount;

        public void SetVisibleLabelCount(int count) => _visibleLabelCount = count;
        public void SetParticipantCount(int count) => _participantCount = count;

        public int CountVisibleLabels(string name, string? contractId) => _visibleLabelCount;
        public int CountParticipants(LabelId labelId) => _participantCount;
    }
}
