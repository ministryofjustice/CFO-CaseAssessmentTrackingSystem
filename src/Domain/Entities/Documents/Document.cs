using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Documents;

public class Document : OwnerPropertyEntity<Guid>, IMayHaveTenant, IAuditTrial
{
    private Document() { } 

    protected Document(string title, string description, DocumentType documentType)
    {
        Id = Guid.CreateVersion7();
        Title = title;
        IsPublic = false;
        Description = description;
        DocumentType = documentType;

        AddDomainEvent(new DocumentCreatedDomainEvent(this));
    }


    public static Document Create(string title, string description, DocumentType documentType)
    {
        return new(title, description, documentType);
    }

    public Document SetURL(string url)
    {
        URL = url;
        return this;
    }

    public Document SetVersion(string version)
    {
        Version = version;
        return this;
    }

    public string? Download()
    {
        AddDomainEvent(new DocumentDownloadedDomainEvent(this, DateTime.UtcNow));
        return URL;
    }

    public string? Title { get; private set; }
    public string? Description { get; private set; }
    
    //todo: remove this field
    public string? Content { get; private set; }
    public bool IsPublic { get; private set; }
    public string? URL { get; private set; }
    public DocumentType DocumentType { get; private set; } = default!;
    public string? Version {  get; private set; }
    public virtual Tenant? Tenant { get; set; }
    public string? TenantId {get; set;}
}

public enum DocumentType
{
    Document,
    Excel,
    Image,
    PDF,
    Others
}