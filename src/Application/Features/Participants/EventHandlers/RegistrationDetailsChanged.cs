using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RegistrationDetailsChanged(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantRegistrationDetailsChangedDomainEvent>
{
    public async Task Handle(ParticipantRegistrationDetailsChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var participant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == notification.Item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var risk = await unitOfWork.DbContext.Risks
            .Where(r =>  r.ParticipantId == notification.Item.Id) 
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is not null && 
            risk is not null &&
            participant.RiskDue!.HasValue && 
            participant.RiskDue!.Value > DateTime.Today)
        {
            //Only applicable if Risk information exists
            //Set Risk Due date to today, only if it is later than today
            participant.SetRiskDue(DateTime.UtcNow);
            unitOfWork.DbContext.Participants.Update(participant);
        }

    }
}
