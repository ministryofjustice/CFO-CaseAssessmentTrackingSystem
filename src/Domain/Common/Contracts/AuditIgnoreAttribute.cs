namespace Cfo.Cats.Domain.Common.Contracts;

/// <summary>
/// Marks an entity property so the audit interceptor does not record its
/// value in the audit trail (old/new values or affected columns).
/// The property is still persisted normally.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AuditIgnoreAttribute : Attribute
{
}
