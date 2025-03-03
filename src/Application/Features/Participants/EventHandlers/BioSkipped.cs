using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class BioSkipped(IUnitOfWork unitOfWork) : INotificationHandler<BioSkippedDomainEvent>
{
    public async Task Handle(BioSkippedDomainEvent notification, CancellationToken cancellationToken)
    {
        var participant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == notification.Entity.ParticipantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is not null)
        {
            participant.SetBioDue(DateTime.UtcNow.AddDays(70));
            unitOfWork.DbContext.Participants.Update(participant);
        }
    }
}