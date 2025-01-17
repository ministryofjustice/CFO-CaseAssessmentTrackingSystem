using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class AddPathwayPlanOnParticipantCreated(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantCreatedDomainEvent>
{
    public async Task Handle(ParticipantCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var pathwayPlan = PathwayPlan.Create(notification.Item.Id);
        await unitOfWork.DbContext.PathwayPlans.AddAsync(pathwayPlan, cancellationToken);
    }
}
