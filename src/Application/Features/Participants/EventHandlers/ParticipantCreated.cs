using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantCreated(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantCreatedDomainEvent>
{
    public async Task Handle(ParticipantCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Update enrolment history
        var enrolmentHistory = ParticipantEnrolmentHistory.Create(notification.Item.Id, EnrolmentStatus.IdentifiedStatus);
        await unitOfWork.DbContext.ParticipantEnrolmentHistories.AddAsync(enrolmentHistory, cancellationToken);

        // Update location history
        var locationHistory = ParticipantLocationHistory.Create(notification.Item.Id, notification.LocationId, DateTime.UtcNow);
        await unitOfWork.DbContext.ParticipantLocationHistories.AddAsync(locationHistory, cancellationToken);
    }
}
