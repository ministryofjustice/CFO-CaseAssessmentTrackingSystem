using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Documents;

public class GeneratedDocument : Document
{
    private GeneratedDocument() : base(string.Empty, string.Empty, DocumentType.Document) { }

    private GeneratedDocument(string title, string description, string createdBy) : base(title, description, DocumentType.Excel)
    {
        CreatedBy = createdBy;
        AddDomainEvent(new GeneratedDocumentCreatedDomainEvent(this));
    }

    public static GeneratedDocument Create(string title, string description, string createdBy)
    {
        return new(title, description, createdBy);
    }

    public GeneratedDocument WithStatus(DocumentStatus status)
    {
        Status = status;
        return this;
    }

    public GeneratedDocument WithExpiry(DateTime expiresOn)
    {
        ExpiresOn = expiresOn;
        return this;
    }

    public DocumentStatus Status { get; private set; }

    public DateTime ExpiresOn { get; private set; }
}

public enum DocumentStatus
{
    Available,
    Processing,
    Error
}
