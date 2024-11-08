using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class NotifyPqaEnrolmentRejectedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {

        if (notification.From == EnrolmentStatus.SubmittedToAuthorityStatus && notification.To == EnrolmentStatus.SubmittedToProviderStatus)
        {
            const string heading = "Enrolment returned from authority";
            string details = "You have enrolments that have been returned";

            // who did the PQA?
            var qaUser = await unitOfWork.DbContext
                .EnrolmentPqaQueue.AsNoTracking()
                .Where(pqa =>
                    pqa.ParticipantId == notification.Item.Id && pqa.IsAccepted == true && pqa.IsCompleted == true)
                .Select(pqa => pqa.LastModifiedBy!)
                .FirstOrDefaultAsync(cancellationToken);

            Notification? previous = unitOfWork.DbContext.Notifications.FirstOrDefault(
                n => n.Heading == heading
                && n.OwnerId == notification.Item.OwnerId
                && n.ReadDate == null
            );

            previous?.ResetNotificationDate();

            if (previous == null)
            {
                var n = Notification.Create(heading, details, qaUser!);
                n.SetLink($"pages/qa/enrolments/pqa/");
                await unitOfWork.DbContext.Notifications.AddAsync(n, cancellationToken);
            }
        }
    }
}