using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantCreated(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantCreatedDomainEvent>
{
    public async Task Handle(ParticipantCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var history = ParticipantEnrolmentHistory.Create(notification.Item.Id, EnrolmentStatus.PendingStatus);
        await unitOfWork.DbContext.ParticipantEnrolmentHistories.AddAsync(history, cancellationToken);
    }
}
