using Cfo.Cats.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
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

        var entries = context.ChangeTracker.Entries().ToList();
        var resolver = new OwnerResolver(entries);

        foreach (var owned in GetFoldedOwnedEntries(entries))
        {
            // a removed or replaced owned value object is not always visible from its owner
            var owner = resolver.Resolve(owned)?.Owner;
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

        // Owned entities (single or collection items, possibly nested) that are not IAuditable
        // (e.g. Lifetime, Supervisor, ExternalIdentifier) are folded into the audit row of their
        // nearest auditable owner, with a path such as "Lifetime.EndDate" or "Scores[Wellbeing].Score".
        var entries = context.ChangeTracker.Entries().ToList();
        var resolver = new OwnerResolver(entries);

        foreach (var owned in GetFoldedOwnedEntries(entries))
        {
            var resolved = resolver.Resolve(owned);
            if (resolved is null)
            {
                continue;
            }

            var ownerAudit = auditEntries.FirstOrDefault(a => ReferenceEquals(a.Entity, resolved.Owner.Entity)).Audit;
            if (ownerAudit is not null)
            {
                AppendProperties(ownerAudit, owned, resolved.Path, isFolded: true);
            }
        }

        return temporaryAuditEntries;
    }

    private sealed record ResolvedOwner(EntityEntry Owner, string Path);

    private static List<EntityEntry> GetFoldedOwnedEntries(IEnumerable<EntityEntry> entries) =>
        entries
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                && e.Entity is not IAuditable
                && e.Metadata.IsOwned()
                && e.Metadata.FindOwnership() is { PrincipalToDependent: not null })
            .ToList();

    // Resolves owned entries to their nearest IAuditable owner. Built once per save so that owner
    // lookups are indexed by key rather than scanning every tracked entry for each owned entry.
    private sealed class OwnerResolver(IReadOnlyList<EntityEntry> entries)
    {
        private readonly Dictionary<IReadOnlyKey, Dictionary<string, EntityEntry>> _indexes = new();
        private readonly Dictionary<EntityEntry, ResolvedOwner?> _resolved = new();
        private readonly Dictionary<EntityEntry, string> _keyNames = new();
        private readonly Dictionary<(EntityEntry Owner, string Navigation), int> _newItemCounts = new();

        public ResolvedOwner? Resolve(EntityEntry owned)
        {
            if (!_resolved.TryGetValue(owned, out var resolved))
            {
                resolved = _resolved[owned] = ResolveCore(owned);
            }

            return resolved;
        }

        // Walks up the ownership chain to the nearest IAuditable owner, building the path on the way.
        private ResolvedOwner? ResolveCore(EntityEntry owned)
        {
            var path = string.Empty;
            var current = owned;

            while (current.Entity is not IAuditable)
            {
                var ownership = current.Metadata.FindOwnership();
                if (ownership?.PrincipalToDependent is null)
                {
                    return null;
                }

                var owner = FindOwnerEntry(current, ownership);
                if (owner is null)
                {
                    return null;
                }

                var navigation = ownership.PrincipalToDependent.Name;
                var segment = ownership.IsUnique
                    ? navigation
                    : $"{navigation}[{DescribeKey(current, owner, navigation)}]";
                path = path.Length == 0 ? segment : $"{segment}.{path}";

                current = owner;
            }

            return path.Length == 0 ? null : new ResolvedOwner(current, path);
        }

        // Items with a database generated key have no value yet, so number them within their owner
        // to stop two new items in one save from sharing a path.
        private string DescribeKey(EntityEntry entry, EntityEntry owner, string navigation)
        {
            if (_keyNames.TryGetValue(entry, out var name))
            {
                return name;
            }

            var foreignKeys = entry.Metadata.FindOwnership()!.Properties;
            var keyProperties = entry.Metadata.FindPrimaryKey()!.Properties
                .Where(p => !foreignKeys.Contains(p))
                .Select(p => entry.Property(p.Name))
                .ToList();

            if (keyProperties.Any(p => p.IsTemporary))
            {
                var counter = _newItemCounts.GetValueOrDefault((owner, navigation)) + 1;
                _newItemCounts[(owner, navigation)] = counter;
                name = $"new#{counter}";
            }
            else
            {
                name = string.Join(",", keyProperties.Select(p =>
                    (entry.State == EntityState.Deleted ? p.OriginalValue : p.CurrentValue)?.ToString()));
            }

            return _keyNames[entry] = name;
        }

        private EntityEntry? FindOwnerEntry(EntityEntry owned, IReadOnlyForeignKey ownership)
        {
            if (!_indexes.TryGetValue(ownership.PrincipalKey, out var index))
            {
                index = new Dictionary<string, EntityEntry>();
                foreach (var entry in entries.Where(e => ownership.PrincipalEntityType.IsAssignableFrom(e.Metadata)))
                {
                    index.TryAdd(
                        Join(ownership.PrincipalKey.Properties.Select(p => entry.Property(p.Name).CurrentValue)),
                        entry);
                }

                _indexes[ownership.PrincipalKey] = index;
            }

            var key = Join(ownership.Properties.Select(p => owned.State == EntityState.Deleted
                ? owned.Property(p.Name).OriginalValue
                : owned.Property(p.Name).CurrentValue));

            return index.GetValueOrDefault(key);
        }

        private static string Join(IEnumerable<object?> values) =>
            string.Join('\u001f', values.Select(v => v?.ToString()));
    }

    // A Create or Delete row leaves AffectedColumns empty for the owner's own properties, so folded
    // properties follow suit; duplicates arise when an item is replaced (one Deleted, one Added).
    private static void AddAffectedColumn(AuditTrail auditEntry, string propertyName)
    {
        if (auditEntry.AuditType == AuditType.Update && !auditEntry.AffectedColumns!.Contains(propertyName))
        {
            auditEntry.AffectedColumns.Add(propertyName);
        }
    }

    private static void AppendProperties(AuditTrail auditEntry, EntityEntry entry, string? prefix, bool isFolded)
    {
        foreach (var property in entry.Properties)
        {
            if (property.Metadata.PropertyInfo?.IsDefined(typeof(AuditIgnoreAttribute), inherit: true) == true)
            {
                continue;
            }

            // key columns that carry data (e.g. ExternalIdentifier.Value) are kept; only the link back
            // to the owner and database-generated values are noise
            if (isFolded && (property.Metadata.IsForeignKey() || property.IsTemporary))
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
                            AddAffectedColumn(auditEntry, propertyName);
                        }

                        auditEntry.NewValues![propertyName] = property.CurrentValue;
                    }

                    break;

                case EntityState.Deleted:
                    if (property.OriginalValue is not null)
                    {
                        if (isFolded)
                        {
                            AddAffectedColumn(auditEntry, propertyName);
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
                    if (isFolded)
                    {
                        AddAffectedColumn(auditEntry, propertyName);
                    }
                    else
                    {
                        auditEntry.AffectedColumns!.Add(propertyName);
                    }

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
