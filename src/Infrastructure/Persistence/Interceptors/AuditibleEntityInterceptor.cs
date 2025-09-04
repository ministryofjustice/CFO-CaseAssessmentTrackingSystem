using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cfo.Cats.Infrastructure.Persistence.Interceptors;

#nullable disable warnings
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService currentUserService;
    private readonly IDateTime dateTime;
    private List<AuditTrail> temporaryAuditTrailList = new();

    public AuditableEntityInterceptor(ICurrentUserService currentUserService, IDateTime dateTime)
    {
        this.currentUserService = currentUserService;
        this.dateTime = dateTime;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context!);
        temporaryAuditTrailList = TryInsertTemporaryAuditTrail(eventData.Context!);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        var resultValueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await TryUpdateTemporaryPropertiesForAuditTrail(eventData.Context!, cancellationToken)
            .ConfigureAwait(false);
        return resultValueTask;
    }

    private void UpdateEntities(DbContext context)
    {
        var userId = currentUserService.UserId;
        var tenantId = currentUserService.TenantId;

        var changes = context.ChangeTracker.Entries<IAuditable>().ToList();

        foreach (var entry in changes)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = dateTime.Now;
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
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = dateTime.Now;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = userId;
                        softDelete.Deleted = dateTime.Now;
                        entry.State = EntityState.Modified;
                    }

                    break;
                case EntityState.Unchanged:
                    if (entry.HasChangedOwnedEntities())
                    {
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModified = dateTime.Now;
                    }

                    break;
            }
        }
    }

    private List<AuditTrail> TryInsertTemporaryAuditTrail(DbContext context)
    {
        var userId = currentUserService.UserId;
        var tenantId = currentUserService.TenantId;
        context.ChangeTracker.DetectChanges();
        var temporaryAuditEntries = new List<AuditTrail>();

        var changes = context.ChangeTracker.Entries<IAuditable>().ToList();

        foreach (var entry in changes)
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            AuditType auditType = entry.State switch
            {
                EntityState.Deleted => AuditType.Delete,
                EntityState.Modified => AuditType.Update,
                EntityState.Added => AuditType.Create,
                _ => throw new ArgumentOutOfRangeException($"No audit setup for {entry.State}")
            };

            var auditEntry = AuditTrail.Create(entry.Entity.GetType().Name, userId, auditType);
  
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey() && property.CurrentValue is not null)
                {
                    auditEntry.PrimaryKey[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        if (property.CurrentValue is not null)
                        {
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;

                    case EntityState.Deleted:
                        if (property.OriginalValue is not null)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
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
                        auditEntry.AffectedColumns.Add(propertyName);
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        if (property.CurrentValue is not null)
                        {
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                }
            }

            temporaryAuditEntries.Add(auditEntry);
        }

        return temporaryAuditEntries;
    }

    private async Task TryUpdateTemporaryPropertiesForAuditTrail(
        DbContext context,
        CancellationToken cancellationToken = default
    )
    {
        if (temporaryAuditTrailList.Any())
        {
            foreach (var auditEntry in temporaryAuditTrailList)
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

            await context.AddRangeAsync(temporaryAuditTrailList, cancellationToken);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            temporaryAuditTrailList.Clear();
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null
            && r.TargetEntry.Metadata.IsOwned()
            && (
                r.TargetEntry.State == EntityState.Added
                || r.TargetEntry.State == EntityState.Modified
            )
        );
    }
}
