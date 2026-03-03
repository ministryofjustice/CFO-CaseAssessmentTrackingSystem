using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

[RequestAuthorize(Policy = SecurityPolicies.ManageLabels)]
public class AddLabelCommand : IRequest<Result>
{
    [Display(Name = "Name", Description = "The display name of the label. Must be unique within a contract")]
    public required string Name { get; set; }

    [Display(Name="Description", Description = "A textual description of the label. Will appear as a tool tip")]
    public required string Description { get; set; }

    [Display(Name = "Scope", Description = "The scope for adding a label (system labels are not available for user selection)")]
    public required LabelScope Scope { get; set; }

    [Display(Name="Colour", Description = "The colour for the label. More pronounced on Filled labels.")]
    public required AppColour Colour { get; set; }
    
    [Display(Name = "Variant", Description = "Label display mode (Text/Outlined/Filled)")]
    public required AppVariant Variant { get; set; }

    [Display(Name ="Icon", Description = "The icon to display (or None)")]
    public AppIcon AppIcon { get; set; }

    [Display(Name="Contract", Description = "Option contract to limit user applicability and visibility")]
    public string? ContractId { get; set; }
    
}
