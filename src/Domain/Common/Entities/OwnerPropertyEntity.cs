using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class OwnerPropertyEntity<TId> : BaseAuditableEntity<TId>
{
    public virtual ApplicationUser? Owner { get; set; }
    
    public virtual ApplicationUser? Editor { get; set; }
    
    public string? OwnerId { get; set; }
    
    public string? EditorId { get; set; }
}
