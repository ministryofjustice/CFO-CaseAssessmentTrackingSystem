using System.ComponentModel.DataAnnotations;

namespace Cfo.Cats.Application.Features.HelpLinks.DTOs;

public class HelpLinkUrlDto
{
    [Display(Name = "Url", Description = "The link the user is taken to for more information")]
    public string Url { get; set; } = string.Empty;

    [Display(Name = "Display Name", Description = "Optional friendly name shown instead of the raw url (e.g. \"Guidance\"). Leave blank to show the url itself.")]
    public string? DisplayName { get; set; }
}
