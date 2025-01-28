using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ReassignParticipantOnMovement(
    IUnitOfWork unitOfWork,
    ILocationService locationService) : INotificationHandler<ParticipantMovedDomainEvent>
{
    public async Task Handle(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        string? newAssignee = await GetNewAssignee(notification, cancellationToken);
        notification.Item.AssignTo(newAssignee);
    }

    private async Task<string?> GetNewAssignee(ParticipantMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Assume current assignee
        var newAssignee = notification.Item.OwnerId;

        // Transferring from Custody -> Community
        if(notification.From.LocationType.IsCustody && notification.To.LocationType.IsCommunity)
        {
            var priAssignee = await GetActivePRIAssignee(notification.Item.Id, notification.To, cancellationToken);
            newAssignee = priAssignee;
        }

        if (await AssigneeHasAccessToNewLocation(newAssignee, notification.To) is false)
        {
            newAssignee = null;
        }

        return newAssignee;
    }

    private async Task<string?> GetActivePRIAssignee(string participantId, Location to, CancellationToken cancellationToken)
    {
        var pri = await unitOfWork.DbContext.PRIs
            .OrderByDescending(p => p.Created)
            .FirstOrDefaultAsync(
                p => p.ParticipantId == participantId 
                && p.ExpectedReleaseRegionId == to.Id
                && p.IsCompleted == false, cancellationToken);

        return pri?.AssignedTo;
    }

    private async Task<bool> AssigneeHasAccessToNewLocation(string? assigneeId, Location to)
    {
        // Can we load the Owner instead of fetching from db?
        var owner = await unitOfWork.DbContext.Users
            .FindAsync(assigneeId);

        if (owner is null)
        {
            return false;
        }

        return locationService
            .GetVisibleLocations(owner.TenantId!)
            .Any(location => location.Id == to.Id);
    }

}
