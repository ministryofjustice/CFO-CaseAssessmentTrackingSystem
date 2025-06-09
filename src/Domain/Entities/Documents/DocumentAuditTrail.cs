using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Documents;

public class DocumentAuditTrail : BaseEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    DocumentAuditTrail() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    DocumentAuditTrail(Guid documentId, string userId, DocumentAuditTrailRequestType requestType, DateTime occurredOn)
    {
        DocumentId = documentId;
        UserId = userId;
        RequestType = requestType;
        OccurredOn = occurredOn;
    }

    public static DocumentAuditTrail Create(Guid documentId, string userId, DocumentAuditTrailRequestType requestType, DateTime occurredOn) => new(documentId, userId, requestType, occurredOn);

    public Guid DocumentId { get; private set; }
    public string UserId { get; private set; }
    public DocumentAuditTrailRequestType RequestType { get; private set; }
    public DateTime OccurredOn { get; private set; }

    public virtual Document? Document { get; private set; }
    public virtual ApplicationUser? User { get; private set; }
}

public enum DocumentAuditTrailRequestType
{
    DocumentGenerated,
    DocumentDownloaded
}

