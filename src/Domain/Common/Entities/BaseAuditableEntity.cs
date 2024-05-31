using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity
{
    public virtual DateTime? Created { get; set; }

    public virtual int? CreatedBy { get; set; }

    public virtual DateTime? LastModified { get; set; }

    public virtual int? LastModifiedBy { get; set; }
}
