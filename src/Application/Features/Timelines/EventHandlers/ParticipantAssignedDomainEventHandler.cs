using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantAssignedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantAssignedDomainEvent>(currentUserService, unitOfWork)
{
    
    protected override string GetLine1(ParticipantAssignedDomainEvent notification) => notification.FromOwner is null ? "Caseworker assigned" : "Caseworker reassigned";

    protected override string? GetLine2(ParticipantAssignedDomainEvent notification)
    {
        return notification.FromOwner is null
            ? $"To {UnitOfWork.DbContext.Users.AsNoTracking().First(t => t.Id == notification.NewOwner).DisplayName}"
            : $"From {UnitOfWork.DbContext.Users.AsNoTracking().First(t => t.Id == notification.FromOwner).DisplayName}";
    }

    protected override string? GetLine3(ParticipantAssignedDomainEvent notification) =>
        notification.FromOwner is null
            ? null
            : $"To {UnitOfWork.DbContext.Users.First(t => t.Id == notification.NewOwner).DisplayName}";
    
    protected override TimelineEventType GetEventType() => TimelineEventType.Participant;
    protected override string GetParticipantId(ParticipantAssignedDomainEvent notification) => notification.Item.Id;
}
