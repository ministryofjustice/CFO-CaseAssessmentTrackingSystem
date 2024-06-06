using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantTransitionedEventHandler(IApplicationDbContext context) : INotificationHandler<ParticipantTransitionedEvent>
{
    public async Task Handle(ParticipantTransitionedEvent notification, CancellationToken cancellationToken)
    {
        ParticipantEnrolmentHistory history = ParticipantEnrolmentHistory.Create(notification.Item.Id, notification.To);
        context.ParticipantEnrolmentHistories.Add(history);
        await context.SaveChangesAsync(cancellationToken);
    }
}