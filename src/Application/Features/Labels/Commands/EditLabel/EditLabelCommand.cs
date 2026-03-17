using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

[RequestAuthorize(Policy = SecurityPolicies.ManageLabels)]
public class EditLabelCommand : IRequest<Result>
{
    public required LabelId LabelId { get; set; }

    [Display(Name = "Name", Description = "The display name of the label. Must be unique within a contract")]
    public required string NewName { get; set; }

    [Display(Name = "Description", Description = "A textual description of the label. Will appear as a tool tip")]
    public required string NewDescription { get; set; }

    [Display(Name = "Scope", Description = "The scope for adding a label (system labels are not available for user selection)")]
    public required LabelScope NewScope { get; set; }

    [Display(Name = "Colour", Description = "The colour for the label. More pronounced on Filled labels.")]
    public AppColour NewColour { get; set; }

    [Display(Name = "Variant", Description = "Label display mode (Text/Outlined/Filled)")]
    public AppVariant NewVariant { get; set; }

    [Display(Name = "Icon", Description = "The icon to display (or None)")]
    public AppIcon NewAppIcon { get; set; }

}
