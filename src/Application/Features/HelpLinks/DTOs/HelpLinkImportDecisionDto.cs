using Cfo.Cats.Application.Features.HelpLinks.DTOs;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

public class HelpLinkImportDecisionDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<HelpLinkUrlDto> Urls { get; set; } = new();
    public string PageKey { get; set; } = string.Empty;
    public string? TabName { get; set; }
    public Guid? ExistingHelpLinkId { get; set; }
}
