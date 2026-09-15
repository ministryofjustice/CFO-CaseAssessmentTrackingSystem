using Cfo.Cats.Application.Common.Interfaces.Locations;

namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class ParticipantHasBeenInVisibleAreaRecentlyAccessSpecification(IUnitOfWork unitOfWork, ILocationService locationService, ICurrentUserService currentUserService) 
    : AccessValidationSpecification
{
    public override int Order => 10;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier)
    {
        var locations = await unitOfWork.DbContext.ParticipantLocationHistories
                                .Where(h => h.ParticipantId == identifier)
                                .Where(h => h.From >= DateTime.Now.AddMonths(-3).Date)
                                .Select(h => h.LocationId)
                                .Distinct()
                                .ToArrayAsync();

        var isInLocation = locationService.GetVisibleLocations(currentUserService.TenantId!)
            .Select(e => e.Id)
            .Intersect(locations)
            .Any();

        return isInLocation ? AccessGrant.Granted : AccessGrant.NotGranted;
    }
}