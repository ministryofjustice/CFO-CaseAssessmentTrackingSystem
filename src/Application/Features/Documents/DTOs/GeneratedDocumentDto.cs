using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Documents.DTOs;

public class GeneratedDocumentDto
{
    [Description("Id")] public required Guid Id { get; set; }
    [Description("File Name")] public string? Title { get; set; }
    [Description("Description")] public string? Description { get; set; }
    [Description("URL")] public string? URL { get; set; }
    [Description("Document Type")] public DocumentType DocumentType { get; set; } = DocumentType.Document;
    [Description("Expires On")] public DateTime ExpiresOn { get; set; }
    [Description("Status")] public DocumentStatus Status { get; set; }
    [Description("Created Date")] public DateTime Created { get; set; }

    public bool IsReadyForDownload => Status == DocumentStatus.Available;

    class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GeneratedDocument, GeneratedDocumentDto>(MemberList.None);
        }
    }
}
