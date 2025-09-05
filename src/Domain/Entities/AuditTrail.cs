using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cfo.Cats.Domain.Entities;

public class AuditTrail : IEntity<int>
{

    private AuditTrail()
    {
    }

    public static AuditTrail Create(string tableName, string? userId, AuditType auditType, DateTime dateTime)
    {
        return new AuditTrail()
        {
            TableName = tableName,
            UserId = userId,
            DateTime = dateTime,
            AuditType = auditType,
            AffectedColumns = new List<string>(),
            NewValues = new Dictionary<string, object?>(),
            OldValues = new Dictionary<string, object?>()
        };
    }

    public int Id { get; private set; }
    public string? UserId { get; set; }
    public virtual ApplicationUser? Owner { get; private set; }
    public AuditType AuditType { get; private set; }
    public string? TableName { get; private set; }
    public DateTime DateTime { get; private set; }
    public Dictionary<string, object?>? OldValues { get; private set; }
    public Dictionary<string, object?>? NewValues { get; private set; }
    public List<string>? AffectedColumns { get; private set; }
    public Dictionary<string, object> PrimaryKey { get; private set; } = new();
    public List<PropertyEntry> TemporaryProperties { get; private set; } = new();
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
