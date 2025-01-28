using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Entities.PRIs;
using Cfo.Cats.Domain.Events;
using DocumentFormat.OpenXml.Bibliography;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyCommunitySupportWorkerPriAssignedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<PRIAssignedDomainEvent>
{
    public async Task Handle(PRIAssignedDomainEvent pri, CancellationToken cancellationToken)
    {
        if (pri.Entity.AssignedTo!.Length > 0 && pri.Entity.CreatedBy != pri.Entity.AssignedTo)
        {
            const string heading = "PRI assigned";

            string details = "You have been assigned a PRI";

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                && n.OwnerId == pri.Entity.AssignedTo
                && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if (previous is null)
            {
                var n = Notification.Create(heading, details, pri.Entity.AssignedTo);
                //n.SetLink($"/pages/participants/{pri.Entity.ParticipantId}");
                n.SetLink($"pages/participants/pre-release-inventory");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}