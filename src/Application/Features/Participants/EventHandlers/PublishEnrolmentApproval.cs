using Cfo.Cats.Application.Features.Participants.Contracts;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class PublishEnrolmentApproval(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{
    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.To == EnrolmentStatus.ApprovedStatus)
        {
            var integrationEvent = new EnrolmentApprovedIntegrationEvent(notification.Item.Id);
            await unitOfWork.DbContext.InsertOutboxMessage(integrationEvent);
        }
    }
}