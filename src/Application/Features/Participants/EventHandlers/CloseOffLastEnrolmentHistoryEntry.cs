using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class CloseOffLastEnrolmentHistoryEntry(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantEnrolmentHistoryCreatedDomainEvent>
{
    public async Task Handle(ParticipantEnrolmentHistoryCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var last = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .Where(e => 
                    e.ParticipantId == notification.Entity.ParticipantId 
                    && e.From < notification.Entity.From
                    && e.To == null)
            .OrderByDescending(e => e.Created)
            .FirstOrDefaultAsync(cancellationToken);

        last?.SetTo(notification.Entity.From);
    }
}
