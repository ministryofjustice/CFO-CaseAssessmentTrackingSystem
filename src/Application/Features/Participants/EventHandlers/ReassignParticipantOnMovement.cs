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
        var participant = await unitOfWork.DbContext.Participants.FindAsync([notification.Item.Id], cancellationToken);

        if(participant is null)
        {
            return;
        }

        string? newAssignee = await GetNewAssignee(notification);
        participant.AssignTo(newAssignee);
    }

    private async Task<string?> GetNewAssignee(ParticipantMovedDomainEvent notification)
    {
        var newAssignee = notification.Item.OwnerId;

        // Todo: Get PRI Assignee
        if (notification.Item.HasActivePRI())
        {
            newAssignee = string.Empty;
        }

        if (await AssigneeHasAccessToNewLocation(newAssignee, notification.To) is false)
        {
            newAssignee = null;
        }

        return newAssignee;
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
