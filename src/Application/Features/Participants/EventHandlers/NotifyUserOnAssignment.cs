using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class NotifyUserOnAssignment(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        // No need to notify themselves!
        if (currentUserService.UserId == notification.NewOwner)
        {
            return;
        }

        var newAssignee = await unitOfWork.DbContext.Users
            .FindAsync([notification.FromOwner], cancellationToken);

        if(newAssignee is null)
        {
            return;
        }

        var n = Notification.Create(
            heading: "Participant assigned", 
            details: $"{notification.Item.FullName} ({notification.Item.Id}) was assigned to you.", 
            userId: newAssignee.Id)
            .SetLink($"/pages/participants/{notification.Item.Id}");

        await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
    }
}
