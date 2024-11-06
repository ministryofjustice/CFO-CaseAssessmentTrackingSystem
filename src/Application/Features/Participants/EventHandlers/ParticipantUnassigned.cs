using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantUnassigned(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        // No need to notify themselves!
        if (currentUserService.UserId == notification.FromOwner)
        {
            return;
        }

        var oldAssignee = await unitOfWork.DbContext.Users
            .FindAsync(notification.FromOwner);

        if(oldAssignee is null)
        {
            return;
        }

        var n = Notification.Create(
            heading: "Participant unassigned", 
            details: $"{notification.Item.FullName} ({notification.Item.Id}), who you were previously working with, was unassigned from you.", 
            userId: oldAssignee.Id)
            .SetLink($"/pages/participants/{notification.Item.Id}");

        await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
    }
}
