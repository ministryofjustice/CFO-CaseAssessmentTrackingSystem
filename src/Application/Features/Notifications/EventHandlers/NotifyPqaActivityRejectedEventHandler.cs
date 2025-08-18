using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyPqaActivityRejectedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityTransitionedDomainEvent>
{
    public async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.From == ActivityStatus.SubmittedToAuthorityStatus && notification.To == ActivityStatus.SubmittedToProviderStatus)
        {
            const string heading = "Activity returned from authority";
            string details = "You have Activities that have been returned";

            // who did the PQA?
            var qaUser = await unitOfWork.DbContext
                .ActivityPqaQueue.AsNoTracking()
                .Where(pqa =>
                    pqa.ActivityId == notification.Item.Id && pqa.IsAccepted == true && pqa.IsCompleted == true)
                .Select(pqa => pqa.LastModifiedBy!)
                .FirstOrDefaultAsync(cancellationToken);

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
            n => n.Heading == heading
                 && n.OwnerId == qaUser
                 && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if (previous == null)
            {
                var n = Notification.Create(heading, details, qaUser!);
                n.SetLink($"pages/qa/activities/pqa/");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}