using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class RegistrationDetailsChanged(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantRegistrationDetailsChangedDomainEvent>
{
    public async Task Handle(ParticipantRegistrationDetailsChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var risk = await unitOfWork.DbContext.Risks
            .Where(risk => risk.ParticipantId == notification.Item.Id)
            .OrderBy(risk => risk.Created)
            .FirstOrDefaultAsync(cancellationToken);

        if(risk is not null)
        {
            //risk.SetDue(DateTime.UtcNow);
        }

    }
}
