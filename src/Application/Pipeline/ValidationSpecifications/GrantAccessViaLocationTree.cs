using Cfo.Cats.Application.Common.Interfaces.Locations;

namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaLocationTree(IUnitOfWork unitOfWork, ILocationService locationService, ICurrentUserService currentUserService) 
    : AccessValidationSpecification
{
    public override int Order => 11;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier)
    {
        var cutoff = DateTime.Today.AddMonths(-3);
        
        var locations = unitOfWork.DbContext.ParticipantLocationHistories
                        .Where(h => h.ParticipantId == identifier)
                        .Where(h => h.To == null || h.To >= cutoff)
                        .Select(h => h.LocationId!)
                        .Distinct();

        var isInLocation = locationService.GetVisibleLocations(currentUserService.TenantId!)
            .Select(e => e.Id)
            .Intersect(locations)
            .Any();

        return isInLocation ? AccessGrant.Granted: AccessGrant.NotGranted;
    }
}
