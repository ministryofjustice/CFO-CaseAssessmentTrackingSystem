using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class OwnerPropertyEntity<TId> : BaseAuditableEntity<TId>
{
    public virtual ApplicationUser? Owner { get; set; }
    
    public int? OwnerId { get; set; }
}
