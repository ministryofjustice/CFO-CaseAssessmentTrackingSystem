namespace Cfo.Cats.Application.Features.HelpLinks.DTOs;

public class HelpLinkImportPreviewItemDto
{
    public string PageKey { get; set; } = string.Empty;
    public string? TabName { get; set; }
    public HelpLinkExportDto Incoming { get; set; } = new();
    public HelpLinkExportDto? Existing { get; set; }
    public Guid? ExistingHelpLinkId { get; set; }
    public HelpLinkImportMatchStatus Status { get; set; }
    public bool IsConflict => Status == HelpLinkImportMatchStatus.Conflict;
    public bool IsUnchanged => Status == HelpLinkImportMatchStatus.Unchanged;
}
