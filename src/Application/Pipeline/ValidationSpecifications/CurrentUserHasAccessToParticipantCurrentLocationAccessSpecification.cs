using Cfo.Cats.Application.Common.Interfaces.Locations;

namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class CurrentUserHasAccessToParticipantCurrentLocationAccessSpecification(IUnitOfWork unitOfWork, ILocationService locationService, ICurrentUserService currentUserService) 
    : AccessValidationSpecification
{
    public override int Order => 10;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier)
    {
        var location = await unitOfWork.DbContext.Participants
                                .Where(p => p.Id == identifier)
                                .Select(p => p.CurrentLocation.Id)
                                .FirstOrDefaultAsync();

        var isInLocation = locationService.GetVisibleLocations(currentUserService.TenantId!)
            .Any(l => l.Id == location);

        return isInLocation ? AccessGrant.Granted : AccessGrant.NotGranted;
    }
}
