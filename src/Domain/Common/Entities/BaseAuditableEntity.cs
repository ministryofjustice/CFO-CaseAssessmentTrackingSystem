using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditable
{
    [AuditIgnore]
    public virtual DateTime? Created { get; set; }

    [AuditIgnore]
    public virtual string? CreatedBy { get; set; }

    [AuditIgnore]
    public virtual DateTime? LastModified { get; set; }

    [AuditIgnore]
    public virtual string? LastModifiedBy { get; set; }
}
