using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Entities.Documents;

public class Document : OwnerPropertyEntity<Guid>, IMayHaveTenant, IAuditTrial
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public bool IsPublic { get; set; }
    public string? URL { get; set; }
    public DocumentType DocumentType { get; set; } = default!;
    public virtual Tenant? Tenant { get; set; }

    public string? TenantId {get;set;}
}

public enum DocumentType
{
    Document,
    Excel,
    Image,
    PDF,
    Others
}