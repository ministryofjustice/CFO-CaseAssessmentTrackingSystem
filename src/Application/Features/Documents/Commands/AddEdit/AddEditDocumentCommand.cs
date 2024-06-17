using Cfo.Cats.Application.Features.Documents.Caching;
using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommand : ICacheInvalidatorRequest<Result<Guid>>
{

    [Description("Id")]
    public Guid Id { get;set; }

    [Description("Title")] public string? Title { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Is Public")] public bool IsPublic { get; set; }

    [Description("URL")] public string? URL { get; set; }

    [Description("Document Type")] public DocumentType DocumentType { get; set; } = DocumentType.Document;

    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Tenant Name")] public string? TenantName { get; set; }

    [Description("Content")] public string? Content { get; set; }

    public UploadRequest? UploadRequest { get; set; }

    public string CacheKey => DocumentCacheKey.GetDocumentCacheKey(Id);
    public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DocumentDto, AddEditDocumentCommand>(MemberList.None);
            CreateMap<AddEditDocumentCommand, Document>(MemberList.None);
        }
    }

}