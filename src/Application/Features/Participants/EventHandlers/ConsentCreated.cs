using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ConsentCreated(IUnitOfWork unitOfWork):INotificationHandler<ConsentCreatedDomainEvent>
{
    public async Task Handle(ConsentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var participant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == notification.ParticipantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is { Consents.Count: 1 })
        {
            //Only on first Consent submit
            participant.SetRiskDue(notification.ConsentDate.AddDays(14));
            participant.SetBioDue(notification.ConsentDate.AddDays(14));
            unitOfWork.DbContext.Participants.Update(participant);
        }
    }
}