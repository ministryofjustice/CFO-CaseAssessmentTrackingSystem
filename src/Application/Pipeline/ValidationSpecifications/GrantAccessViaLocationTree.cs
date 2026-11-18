using Cfo.Cats.Application.Common.Interfaces.Locations;

namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaLocationTree(IUnitOfWork unitOfWork, ILocationService locationService, ICurrentUserService currentUserService) 
    : AccessValidationSpecification
{
    public override int Order => 11;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.Today.AddMonths(-3);
        
        var locations = unitOfWork.DbContext.ParticipantLocationHistories
                        .Where(h => h.ParticipantId == identifier)
                        .Where(h => h.To == null || h.To >= cutoff)
                        .Select(h => h.LocationId!)
                        .Distinct();

        var visibleLocationIds = locationService.GetVisibleLocations(currentUserService.TenantId!)
            .Select(e => e.Id)
            .ToArray();

        var isInLocation = await locations.AnyAsync(id => visibleLocationIds.Contains(id), cancellationToken);

        return isInLocation ? AccessGrant.Granted: AccessGrant.NotGranted;
    }

    protected override async Task<IReadOnlyDictionary<string, AccessGrant>> CheckAccessManyAsync(
        IReadOnlyCollection<string> identifiers,
        CancellationToken cancellationToken)
    {
        var cutoff = DateTime.Today.AddMonths(-3);
        var visibleLocationIds = locationService.GetVisibleLocations(currentUserService.TenantId!)
            .Select(e => e.Id)
            .ToArray();
        var ids = identifiers.ToArray();

        var grantedIds = (await unitOfWork.DbContext.ParticipantLocationHistories
                .Where(h => ids.Contains(h.ParticipantId))
                .Where(h => h.To == null || h.To >= cutoff)
                .Where(h => visibleLocationIds.Contains(h.LocationId))
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
