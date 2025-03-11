using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class BioSubmitted(IUnitOfWork unitOfWork) : INotificationHandler<BioSubmittedDomainEvent>
{
    public async Task Handle(BioSubmittedDomainEvent notification, CancellationToken cancellationToken)
    {
        var participant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == notification.Item.ParticipantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is not null)
        {
            participant.SetBioDue(DateTime.UtcNow.AddDays(70));
            unitOfWork.DbContext.Participants.Update(participant);
        }
    }
}