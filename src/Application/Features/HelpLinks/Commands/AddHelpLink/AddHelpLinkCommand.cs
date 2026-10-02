using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.AddHelpLink;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class AddHelpLinkCommand : ICommand<Result>
{
    [Display(Name = "Title", Description = "The title shown at the top of the help popup")]
    public required string Title { get; set; }

    [Display(Name = "Description", Description = "The help text shown in the popup")]
    public required string Description { get; set; }

    [Display(Name = "Urls", Description = "The link(s) the user is taken to for more information")]
    public List<HelpLinkUrlDto> Urls { get; set; } = new();

    [Display(Name = "Page", Description = "The page this help link should appear on")]
    public required string PageKey { get; set; }

    [Display(Name = "Tab", Description = "The specific tab (if any) this help link should appear on")]
    public string? TabName { get; set; }
}
