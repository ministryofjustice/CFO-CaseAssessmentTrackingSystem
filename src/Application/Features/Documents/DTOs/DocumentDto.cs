using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Documents.DTOs;

[Description("Documents")]
public class DocumentDto
{
    [Description("Id")] public Guid? Id { get; set; }

    [Description("Title")] public string? Title { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Is Public")] public bool IsPublic { get; set; }

    [Description("URL")] public string? URL { get; set; }

    [Description("Document Type")] public DocumentType DocumentType { get; set; } = DocumentType.Document;

    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Tenant Name")] public string? TenantName { get; set; }

    [Description("Content")] public string? Content { get; set; }
    [Description("Created By")] public string CreatedBy { get; set; } = string.Empty;
    [Description("Created Date")] public DateTime Created { get; set; }

    [Description("Owner")] public ApplicationUserDto? Owner { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Document, DocumentDto>(MemberList.None)
                .ForMember(x => x.TenantName, s => s.MapFrom(y => y.Tenant!.Name));
            CreateMap<DocumentDto, Document>(MemberList.None)
                .ForMember(x => x.Tenant, s => s.Ignore())
                .ForMember(x => x.Owner, s => s.Ignore());
        }
    }
}