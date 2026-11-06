using Cfo.Cats.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cfo.Cats.Infrastructure.Persistence.Interceptors;

#nullable disable warnings
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private List<AuditTrail> _temporaryAuditTrailList = new();

    public AuditableEntityInterceptor(ICurrentUserService currentUserService, IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context!);
        _temporaryAuditTrailList = TryInsertTemporaryAuditTrail(eventData.Context!);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        var resultValueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await TryUpdateTemporaryPropertiesForAuditTrail(eventData.Context!, cancellationToken);
        return resultValueTask;
    }

    private void UpdateEntities(DbContext context)
    {
        var userId = _currentUserService.UserId;
        var tenantId = _currentUserService.TenantId; // top level;

        context.ChangeTracker.DetectChanges();

        foreach (var owned in GetFoldedOwnedEntries(context))
        {
            // a removed or replaced owned value object is not always visible from its owner
            var owner = FindOwnerEntry(context, owned);
            if (owner is { State: EntityState.Unchanged, Entity: IAuditable auditableOwner })
            {
                auditableOwner.LastModifiedBy = userId;
                auditableOwner.LastModified = _dateTime.Now;
            }
        }

        var changes = context.ChangeTracker.Entries<IAuditable>().ToList();

        foreach (var entry in changes)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // if the CurrentUserService is not null, go off the entity created by to allow
                    // system users to set the created by manually
                    entry.Entity.CreatedBy = userId ?? entry.Entity.CreatedBy;
                    entry.Entity.Created = _dateTime.Now;
                    if (entry.Entity is IMustHaveTenant mustTenant && string.IsNullOrEmpty(mustTenant.TenantId))
                    {
                        mustTenant.TenantId = tenantId!;
                    }

                    if (entry.Entity is IMayHaveTenant mayTenant && string.IsNullOrEmpty(mayTenant.TenantId))
                    {
                        mayTenant.TenantId = tenantId;
                    }

                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId ?? entry.Entity.LastModifiedBy ;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
                case EntityState.Unchanged:
                    if (entry.HasChangedOwnedEntities())
                    {
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModified = _dateTime.Now;
                    }
                    break;
            }
        }
    }

    private List<AuditTrail> TryInsertTemporaryAuditTrail(DbContext context)
    {
        var userId = _currentUserService.UserId;

        context.ChangeTracker.DetectChanges();
        var temporaryAuditEntries = new List<AuditTrail>();
        var auditEntries = new List<(object Entity, AuditTrail Audit)>();

        var changes = context.ChangeTracker.Entries<IAuditable>().ToList();

        changes.RemoveAll(c => c.Entity is IShallowAuditable);

        foreach (var entry in changes)
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            var auditType = entry.State switch
            {
                EntityState.Deleted => AuditType.Delete,
                EntityState.Modified => AuditType.Update,
                EntityState.Added => AuditType.Create,
                _ => throw new ArgumentOutOfRangeException($"No audit setup for {entry.State}")
            };

            userId ??= auditType switch
            {
                AuditType.Create => entry.Entity.CreatedBy,
                AuditType.Update => entry.Entity.LastModifiedBy,
                AuditType.Delete => entry.Entity.LastModifiedBy,
                _ => null,
            };

            var auditEntry = AuditTrail.Create(entry.Entity.GetType().Name, userId, auditType, _dateTime.Now);
  
            AppendProperties(auditEntry, entry, prefix: null, isFolded: false);

            temporaryAuditEntries.Add(auditEntry);
            auditEntries.Add((entry.Entity, auditEntry));
        }

        // Owned value objects (e.g. Lifetime) and non-auditable owned entities (e.g. PersonalDetail)
        // are not IAuditable, so fold their changes into the audit row of their owner.
        foreach (var owned in GetFoldedOwnedEntries(context))
        {
            var owner = FindOwnerEntry(context, owned);
            var navigation = owned.Metadata.FindOwnership()?.PrincipalToDependent?.Name;
            var ownerAudit = auditEntries.FirstOrDefault(a => ReferenceEquals(a.Entity, owner?.Entity)).Audit;

            if (ownerAudit is not null && navigation is not null)
            {
                AppendProperties(ownerAudit, owned, navigation, isFolded: true);
            }
        }

        return temporaryAuditEntries;
    }

    private static List<EntityEntry> GetFoldedOwnedEntries(DbContext context) =>
        context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                && e.Entity is not IAuditable
                && e.Metadata.IsOwned()
                && e.Metadata.FindOwnership() is { IsUnique: true, PrincipalToDependent: not null })
            .ToList();

    private static EntityEntry? FindOwnerEntry(DbContext context, EntityEntry owned)
    {
        var ownership = owned.Metadata.FindOwnership();
        if (ownership is null)
        {
            return null;
        }

        var keyValues = ownership.Properties
            .Select(p => owned.State == EntityState.Deleted
                ? owned.Property(p.Name).OriginalValue
                : owned.Property(p.Name).CurrentValue)
            .ToList();

        return context.ChangeTracker.Entries().FirstOrDefault(e =>
            ownership.PrincipalEntityType.IsAssignableFrom(e.Metadata)
            && ownership.PrincipalKey.Properties
                .Select(p => e.Property(p.Name).CurrentValue)
                .SequenceEqual(keyValues));
    }

    private static void AppendProperties(AuditTrail auditEntry, EntityEntry entry, string? prefix, bool isFolded)
    {
        foreach (var property in entry.Properties)
        {
            if (property.Metadata.PropertyInfo?.IsDefined(typeof(AuditIgnoreAttribute), inherit: true) == true)
            {
                continue;
            }

            if (isFolded && (property.Metadata.IsPrimaryKey() || property.Metadata.IsForeignKey()))
            {
                continue;
            }

            if (property.IsTemporary)
            {
                auditEntry.TemporaryProperties.Add(property);
                continue;
            }

            var propertyName = prefix is null ? property.Metadata.Name : $"{prefix}.{property.Metadata.Name}";
            if (!isFolded && property.Metadata.IsPrimaryKey() && property.CurrentValue is not null)
            {
                auditEntry.PrimaryKey[propertyName] = property.CurrentValue;
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    if (property.CurrentValue is not null)
                    {
                        if (isFolded)
                        {
                            auditEntry.AffectedColumns!.Add(propertyName);
                        }

                        auditEntry.NewValues![propertyName] = property.CurrentValue;
                    }

                    break;

                case EntityState.Deleted:
                    if (property.OriginalValue is not null)
                    {
                        if (isFolded)
                        {
                            auditEntry.AffectedColumns!.Add(propertyName);
                        }

                        auditEntry.OldValues![propertyName] = property.OriginalValue;
                    }

                    break;

                case EntityState.Modified
                    when property.IsModified
                        && (
                            (
                                property.OriginalValue is null
                                && property.CurrentValue is not null
                            )
                            || (
                                property.OriginalValue is not null
                                && property.OriginalValue.Equals(property.CurrentValue) == false
                            )
                        ):
                    auditEntry.AffectedColumns!.Add(propertyName);
                    auditEntry.OldValues![propertyName] = property.OriginalValue;
                    if (property.CurrentValue is not null)
                    {
                        auditEntry.NewValues![propertyName] = property.CurrentValue;
                    }

                    break;
            }
        }
    }

    private async Task TryUpdateTemporaryPropertiesForAuditTrail(
        DbContext context,
        CancellationToken cancellationToken = default
    )
    {
        if (_temporaryAuditTrailList.Any())
        {
            foreach (var auditEntry in _temporaryAuditTrailList)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is not null)
                    {
                        auditEntry.PrimaryKey[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (auditEntry.NewValues is not null && prop.CurrentValue is not null)
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
            }

            await context.AddRangeAsync(_temporaryAuditTrailList, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            _temporaryAuditTrailList.Clear();
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) => entry.References.Any(r =>
                                                                                   r.TargetEntry != null
                                                                                   && r.TargetEntry.Metadata.IsOwned()
                                                                                   && (
                                                                                       r.TargetEntry.State == EntityState.Added
                                                                                       || r.TargetEntry.State == EntityState.Modified
                                                                                   )
        );
}
