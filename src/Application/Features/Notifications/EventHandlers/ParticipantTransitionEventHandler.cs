using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class ParticipantTransitionEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.From == EnrolmentStatus.SubmittedToProviderStatus && notification.To == EnrolmentStatus.EnrollingStatus)
        {

            string heading = "PQA returned Enrolment";
            string details = $"Enrolment for {notification.Item.FirstName} {notification.Item.LastName} has been returned by PQA.";
            //notify support worker
            var n = Notification.Create(heading, details, notification.Item.OwnerId!);
            n.SetLink($"/pages/participants/{notification.Item.Id}");
            await unitOfWork.DbContext.Notifications.AddAsync(n);
        }
        if (notification.From == EnrolmentStatus.SubmittedToAuthorityStatus && notification.To == EnrolmentStatus.SubmittedToProviderStatus)
        {
            var pqaQuery = await unitOfWork.DbContext.EnrolmentPqaQueue.
                Where(pqa => pqa.ParticipantId == notification.Item.Id && pqa.IsAccepted==true && pqa.IsCompleted==true).
                OrderByDescending(pqa => pqa.Created).
                FirstOrDefaultAsync(cancellationToken);
            string heading = "2nd Pass returned Enrolment";
            string details = $"Enrolment for {notification.Item.FirstName} {notification.Item.LastName} has been returned by 2nd Pass.";
            //notify PQA
            var n = Notification.Create(heading, details, pqaQuery!.LastModifiedBy!);
            n.SetLink($"/pages/participants/{notification.Item.Id}");
            await unitOfWork.DbContext.Notifications.AddAsync(n);
        }
        if (notification.To == EnrolmentStatus.ApprovedStatus)
        {
            string heading = "Enrolment Approved";
            string details = $"Enrolment for {notification.Item.FirstName} {notification.Item.LastName} has been approved.";
            //notify support worker
            var n = Notification.Create(heading, details, notification.Item.OwnerId!);
            n.SetLink($"/pages/participants/{notification.Item.Id}");
            await unitOfWork.DbContext.Notifications.AddAsync(n);
        }
    }
}