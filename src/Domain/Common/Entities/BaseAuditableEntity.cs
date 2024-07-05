using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditable
{
    public virtual DateTime? Created { get; set; }

    public virtual string? CreatedBy { get; set; }

    public virtual DateTime? LastModified { get; set; }

    public virtual string? LastModifiedBy { get; set; }
}
