using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.EditHelpLink;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class EditHelpLinkCommand : ICommand<Result>
{
    public required Guid HelpLinkId { get; set; }

    [Display(Name = "Title", Description = "The title shown at the top of the help popup")]
    public required string NewTitle { get; set; }

    [Display(Name = "Description", Description = "The help text shown in the popup")]
    public required string NewDescription { get; set; }

    [Display(Name = "Urls", Description = "The link(s) the user is taken to for more information")]
    public List<HelpLinkUrlDto> NewUrls { get; set; } = new();

    [Display(Name = "Page", Description = "The page this help link should appear on")]
    public required string NewPageKey { get; set; }

    [Display(Name = "Tab", Description = "The specific tab (if any) this help link should appear on")]
    public string? NewTabName { get; set; }
}
