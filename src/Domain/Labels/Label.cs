using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels.Events;
using Cfo.Cats.Domain.Labels.Rules;

namespace Cfo.Cats.Domain.Labels;

public class Label : BaseAuditableEntity<LabelId>
{
    private readonly List<LabelContract> _contracts = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Label()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Label(
        string name,
        string description,
        LabelScope scope,
        AppColour colour,
        AppVariant variant,
        AppIcon appIcon,
        IEnumerable<string> contractIds,
        ILabelCounter labelCounter)
    {
        CheckRule(new NameCannotBeNullOrEmptyRule(name));
        CheckRule(new NameMustBeValidLength(name));
        CheckRule(new NameMustBeUniqueRule(labelCounter, name));
        CheckRule(new DescriptionMustBeValidLength(description));
        CheckRule(new DescriptionCannotBeNullOrEmpty(description));

        Id = new  LabelId(Guid.CreateVersion7());
        Name = name;
        Description =  description;
        Scope = scope;
        Colour = colour;
        Variant = variant;
        AppIcon = appIcon;

        SetContracts(contractIds);

        AddDomainEvent(new LabelCreatedDomainEvent(this));
    }

    public static Label Create(
        string name,
        string description,
        LabelScope scope,
        AppColour colour,
        AppVariant variant,
        AppIcon appIcon,
        IEnumerable<string> contractIds,
        ILabelCounter labelCounter)
        => new (name, description, scope, colour, variant, appIcon, contractIds, labelCounter);

    /// <summary>
    /// The name of the label. Used for display and filtering.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// A longer description of the label and its intended use
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// The scope for the label (are we added via the system or the user?)
    /// </summary>
    public LabelScope Scope {get; private set;}

    /// <summary>
    /// The colour for the label
    /// </summary>
    public AppColour Colour { get; private set; }

    public AppVariant Variant { get; private set; }

    /// <summary>
    /// The icon for the label
    /// </summary>
    public AppIcon AppIcon { get; private set; }

    /// <summary>
    /// The contracts this label is associated with. A label is only visible for the
    /// contracts it is associated with; a label with no contracts is visible nowhere.
    /// </summary>
    public IReadOnlyCollection<LabelContract> Contracts => _contracts.AsReadOnly();

    public Label Edit(
        string name,
        string description,
        LabelScope scope,
        AppColour colour,
        AppVariant variant,
        AppIcon appIcon,
        IEnumerable<string> contractIds,
        ILabelCounter labelCounter) =>
            EditName(name, labelCounter)
                .EditDescription(description)
                .EditColour(colour)
                .EditVariant(variant)
                .EditAppIcon(appIcon)
                .EditScope(scope)
                .EditContracts(contractIds);

    private Label EditName(string name, ILabelCounter labelCounter)
    {
        if (!Equals(Name, name))
        {
            CheckRule(new NameMustBeValidLength(name));
            CheckRule(new LabelCannotBeRenamedIfExistingLabelExists(name, Name, labelCounter));
            AddDomainEvent(new LabelRenamedDomainEvent(Id, Name, name ));
            Name = name;
        }

        return this;
    }

    private Label EditColour(AppColour colour)
    {
        if (Colour != colour)
        {
            AddDomainEvent(new LabelColourChangedDomainEvent(Id, Colour, colour));
            Colour = colour;
        }

        return this;
    }

    private Label EditVariant(AppVariant variant)
    {
        if (Variant != variant)
        {
            AddDomainEvent(new LabelVariantChangedDomainEvent(Id, Variant, variant));
            Variant = variant;
        }

        return this;
    }

    private Label EditDescription(string newDescription)
    {
        if (Description != newDescription)
        {
            CheckRule(new DescriptionMustBeValidLength(newDescription));
            AddDomainEvent(new LabelDescriptionChangedDomainEvent(Id, Description, newDescription));
            Description = newDescription;
        }

        return this;
    }

    private Label EditScope(LabelScope newScope)
    {
        if(Scope != newScope)
        {
            AddDomainEvent(new LabelScopeChangedDomainEvent(Id, Scope, newScope));
            Scope = newScope;
        }
        return this;
    }

    private Label EditAppIcon(AppIcon appIcon)
    {
        if (AppIcon != appIcon)
        {
            AddDomainEvent(new LabelAppIconChangedDomainEvent(Id, AppIcon, appIcon));
            AppIcon = appIcon;
        }

        return this;
    }

    private Label EditContracts(IEnumerable<string> contractIds)
    {
        SetContracts(contractIds);
        return this;
    }

    private void SetContracts(IEnumerable<string> contractIds)
    {
        var desired = contractIds
            .Where(id => string.IsNullOrWhiteSpace(id) == false)
            .Distinct()
            .ToArray();

        _contracts.RemoveAll(existing => desired.Contains(existing.ContractId) == false);

        foreach (var contractId in desired)
        {
            if (_contracts.Any(existing => existing.ContractId == contractId) == false)
            {
                _contracts.Add(new LabelContract(contractId));
            }
        }
    }

    public void Delete(ILabelCounter labelCounter)
    {
        CheckRule(new LabelCannotBeDeletedIfParticipantsAreLinked(Id, labelCounter));

        // we raise an event that deletion is valid.
        AddDomainEvent(new LabelDeletedDomainEvent(this));
    }
}
