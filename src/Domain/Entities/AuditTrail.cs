using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;

namespace Cfo.Cats.Domain.Entities;

public class AuditTrail : IEntity<int>
{

    public int Id { get; set; }
    public int? UserId { get; set; }
    public virtual ApplicationUser? Owner { get; set; }
    public AuditType AuditType { get; set; }
    public string? TableName { get; set; }
    public DateTime DateTime { get; set; }
    public Dictionary<string, object?>? OldValues { get; set; }
    public Dictionary<string, object?>? NewValues { get; set; }
    public List<string>? AffectedColumns { get; set; }
    public Dictionary<string, object> PrimaryKey { get; set; } = new();
    public List<PropertyEntry> TemporaryProperties { get; } = new();
    public bool HasTemporaryProperties => TemporaryProperties.Any();

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => new List<DomainEvent>().AsReadOnly();

    public void ClearDomainEvents() { }
}

public enum AuditType
{
    None,
    Create,
    Update,
    Delete
}
