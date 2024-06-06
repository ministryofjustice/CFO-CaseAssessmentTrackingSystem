using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantCreatedEventHandler(IApplicationDbContext context) : INotificationHandler<ParticipantCreatedDomainEvent>
{
    public async Task Handle(ParticipantCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        context.ParticipantEnrolmentHistories.Add(ParticipantEnrolmentHistory.Create(notification.Item.Id, EnrolmentStatus.PendingStatus));
        await context.SaveChangesAsync(cancellationToken);
    }
}
