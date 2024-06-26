namespace Cfo.Cats.Application.Features.Documents.DTOs;

public class DownloadDocumentDto
{
    public required Stream FileStream { get; set; }
    public required string FileExtension { get; set; }
    public required string FileName { get; set; }
}
