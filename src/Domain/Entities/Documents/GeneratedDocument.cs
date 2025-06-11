using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Documents;

public class GeneratedDocument : Document
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private GeneratedDocument() : base(string.Empty, string.Empty, DocumentType.Document) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private GeneratedDocument(DocumentTemplate template, string title, string description, string createdBy, string tenantId, string? searchCriteria = null) : base(title, description, DocumentType.Excel)
    {
        Template = template;
        CreatedBy = createdBy;
        TenantId = tenantId;
        SearchCriteriaUsed = searchCriteria;
        Status = DocumentStatus.Processing;
        ExpiresOn = DateTime.Today.AddDays(8);
        AddDomainEvent(new GeneratedDocumentCreatedDomainEvent(this));
    }

    public static GeneratedDocument Create(DocumentTemplate template, string title, string description, string createdBy, string tenantId, string? searchCriteria = null)
    {
        return new(template, title, description, createdBy, tenantId, searchCriteria);
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

    public string? SearchCriteriaUsed { get; private set; }

    public DocumentStatus Status { get; private set; }

    public DateTime ExpiresOn { get; private set; }

    public DocumentTemplate Template { get; private set; }
}

public enum DocumentStatus
{
    Available,
    Processing,
    Error
}

public class DocumentTemplate : SmartEnum<DocumentTemplate>
{
    DocumentTemplate(string name, int value) : base(name, value) { }

    public static readonly DocumentTemplate CaseWorkload = new(nameof(CaseWorkload), 0);
    public static readonly DocumentTemplate KeyValues = new(nameof(KeyValues), 1);
    public static readonly DocumentTemplate RiskDue = new(nameof(RiskDue), 2);
    public static readonly DocumentTemplate RiskDueAggregate = new(nameof(RiskDueAggregate), 3);
    public static readonly DocumentTemplate Participants = new(nameof(Participants), 4);
    public static readonly DocumentTemplate CumulativeFigures = new(nameof(CumulativeFigures), 100);
}
