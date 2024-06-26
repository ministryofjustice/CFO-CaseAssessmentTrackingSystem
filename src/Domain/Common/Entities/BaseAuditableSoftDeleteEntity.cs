using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class BaseAuditableSoftDeleteEntity<T> : BaseAuditableEntity<T>, ISoftDelete
{
    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}
