using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels.Events;
using Cfo.Cats.Domain.Labels.Rules;

namespace Cfo.Cats.Domain.Labels;

public class Label : BaseAuditableEntity<LabelId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Label()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Label(
        string name, 
        string description, 
        AppColour colour, 
        AppVariant variant, 
        string? contractId, 
        ILabelCounter labelCounter)
    {
        CheckRule(new LabelCannotBeNullOrEmptyRule(name));
        CheckRule(new LabelMustBeBetweenTwoAndFifteenCharactersRule(name));
        CheckRule(new LabelNamesMustBeUniqueAtContractLevelRule(labelCounter, name, contractId));
        
        Id = new  LabelId(Guid.CreateVersion7());
        Name = name;
        Description =  description;
        Colour = colour;
        Variant = variant;
        ContractId = contractId;
        
        AddDomainEvent(new LabelCreatedDomainEvent(this));
    }
    
    public static Label Create(
        string name, 
        string description, 
        AppColour colour, 
        AppVariant variant,
        string? contractId,
        ILabelCounter labelCounter)
        => new (name, description, colour, variant, contractId, labelCounter);
    
    /// <summary>
    /// The name of the label. Used for display and filtering.
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// A longer description of the label and its intended use
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// The colour for the label
    /// </summary>
    public AppColour Colour { get; private set; }
    
    public AppVariant Variant { get; private set; }
    
    /// <summary>
    /// The contract id if this label is linked to one
    /// </summary>
    public string? ContractId { get; private set; }

    public Label Edit(
        string name, 
        string description, 
        AppColour colour,
        AppVariant variant) => 
            EditName(name)
                .EditDescription(description)
                .EditColour(colour)
                .EditVariant(variant);

    private Label EditName(string name)
    {
        if (!Equals(Name, name))
        {
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
            AddDomainEvent(new LabelDescriptionChangedDomainEvent(Id, Description, newDescription));
            Description = newDescription;
        }
        
        return this;
    }

    public void Delete(DomainUser domainUser, ILabelCounter labelCounter)
    {
        CheckRule(new GlobalRulesCanOnlyBeDeletedByInternalUsersRule(ContractId, domainUser));
        CheckRule(new LabelCannotBeDeletedIfParticipantsAreLinked(Id, labelCounter));
        
        AddDomainEvent(new LabelDeletedDomainEvent(this));
    }
}