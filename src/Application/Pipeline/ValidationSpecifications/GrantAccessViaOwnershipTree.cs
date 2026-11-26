namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaOwnershipTree(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 10;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.Today.AddMonths(-3);
        
        var owners = unitOfWork.DbContext.ParticipantOwnershipHistories
                        .Where(h => h.ParticipantId == identifier)
                        .Where(h => h.To == null || h.To >= cutoff)
                        .Select(h => h.TenantId!)
                        .Distinct();

        return await owners.AnyAsync(x => x.StartsWith(currentUserService.TenantId!), cancellationToken)
            ? AccessGrant.Granted : AccessGrant.NotGranted;

    }

    protected override async Task<IReadOnlyDictionary<string, AccessGrant>> CheckAccessManyAsync(
        IReadOnlyCollection<string> identifiers,
        CancellationToken cancellationToken)
    {
        var tenantId = currentUserService.TenantId!;
        var cutoff = DateTime.Today.AddMonths(-3);
        var ids = identifiers.ToArray();

        var grantedIds = (await unitOfWork.DbContext.ParticipantOwnershipHistories
                .Where(h => ids.Contains(h.ParticipantId))
                .Where(h => h.To == null || h.To >= cutoff)
                .Where(h => h.TenantId != null && h.TenantId.StartsWith(tenantId))
                .Select(h => h.ParticipantId)
                .Distinct()
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return identifiers.ToDictionary(
            id => id,
            id => grantedIds.Contains(id) ? AccessGrant.Granted : AccessGrant.NotGranted,
            StringComparer.OrdinalIgnoreCase);
    }
                 
}