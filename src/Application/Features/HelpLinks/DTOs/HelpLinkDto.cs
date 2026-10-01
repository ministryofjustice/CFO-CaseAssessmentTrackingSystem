namespace Cfo.Cats.Application.Features.HelpLinks.DTOs;

public class HelpLinkDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<HelpLinkUrlDto> Urls { get; set; } = new();
    public string PageKey { get; set; } = string.Empty;
    public string? TabName { get; set; }
}
