using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantAssignedDomainEventHandler(ICurrentUserService currentUserService, IApplicationDbContext context) : TimelineNotificationHandler<ParticipantAssignedDomainEvent>(currentUserService, context)
{
    private readonly IApplicationDbContext _context = context;
    protected override string GetLine1(ParticipantAssignedDomainEvent notification) => notification.FromOwner is null ? "Caseworker assigned" : "Caseworker reassigned";

    protected override string? GetLine2(ParticipantAssignedDomainEvent notification) =>
        notification.FromOwner is null
            ? $"To {_context.Users.First(t => t.Id == notification.NewOwner).DisplayName}"
            : $"From {_context.Users.First(t => t.Id == notification.FromOwner).DisplayName}";
    
    protected override string? GetLine3(ParticipantAssignedDomainEvent notification) =>
        notification.FromOwner is null
            ? null
            : $"To {_context.Users.First(t => t.Id == notification.NewOwner).DisplayName}";
    
    protected override TimelineEventType GetEventType() => TimelineEventType.Participant;
    protected override string GetParticipantId(ParticipantAssignedDomainEvent notification) => notification.Item.Id;
}
