using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Participants.Services;

//Decides whether a participant who has returned to a location they recently left should have their previous owner restored
public interface IReturnedParticipantOwnerResolver
{
    // Returns the id of the previous owner to restore, or null if no restoration applies.
    Task<string?> GetOwnerToRestore(string participantId, Location from, Location to, CancellationToken cancellationToken);
}

public class ReturnedParticipantOwnerResolver(
    IUnitOfWork unitOfWork,
    ILocationService locationService,
    IDateTime dateTime) : IReturnedParticipantOwnerResolver
{
    // How long after leaving a location a participant can return and have their previous owner restored.
    public static readonly TimeSpan ReturnWindow = TimeSpan.FromDays(7);

    // Unassignment and location history are written in the same movement, but with independently captured timestamps, so allow for a tolerance
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(1);

    public async Task<string?> GetOwnerToRestore(string participantId, Location from, Location to, CancellationToken cancellationToken)
    {
        var cutoff = dateTime.Now - ReturnWindow;

        // When did they last leave the location they are returning to?
        var leftAt = await unitOfWork.DbContext.ParticipantLocationHistories
            .Where(h => h.ParticipantId == participantId && h.LocationId == to.Id && h.To != null)
            .OrderByDescending(h => h.To)
            .Select(h => h.To)
            .FirstOrDefaultAsync(cancellationToken);

        if (leftAt is null || leftAt < cutoff)
        {
            return null;
        }

        var previousOwnerId = await GetOwnerAtDeparture(participantId, leftAt.Value, cancellationToken);

        if (previousOwnerId is null)
        {
            return null;
        }

        return await IsActiveWithAccess(previousOwnerId, to)
            ? previousOwnerId
            : null;
    }

    private async Task<string?> GetOwnerAtDeparture(string participantId, DateTime leftAt, CancellationToken cancellationToken)
    {
        // The owner whose ownership was closed off by the movement out.
        // If it wasn't closed then they were never unassigned and there is nothing to restore.
        var earliest = leftAt - Tolerance;
        var latest = leftAt + Tolerance;

        return await unitOfWork.DbContext.ParticipantOwnershipHistories
            .Where(h => h.ParticipantId == participantId
                     && h.OwnerId != null
                     && h.From <= latest
                     && h.To >= earliest
                     && h.To <= latest)
            .OrderByDescending(h => h.From)
            .Select(h => h.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> IsActiveWithAccess(string ownerId, Location to)
    {
        var owner = await unitOfWork.DbContext.Users.FindAsync(ownerId);

        if (owner is null || owner.Status != UserStatus.Active || owner.TenantId is null)
        {
            return false;
        }

        return locationService
            .GetVisibleLocations(owner.TenantId)
            .Any(location => location.Id == to.Id);
    }
}
