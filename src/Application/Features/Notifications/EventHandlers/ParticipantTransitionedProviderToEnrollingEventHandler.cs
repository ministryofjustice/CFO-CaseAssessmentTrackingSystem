using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Notifications.EventHandlers;

public class ParticipantTransitionedProviderToEnrollingEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
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
    }
}



