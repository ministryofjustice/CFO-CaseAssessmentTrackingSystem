using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantIdentifierChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantIdentifierChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantIdentifierChangedDomainEvent notification) => $"Identifier {notification.From.Type.Name} changed";

    protected override string? GetLine2(ParticipantIdentifierChangedDomainEvent notification) => $"To {notification.To.Value}";
    protected override string? GetLine3(ParticipantIdentifierChangedDomainEvent notification) => $"From {notification.From.Value}";

    protected override string GetParticipantId(ParticipantIdentifierChangedDomainEvent notification) => notification.Item.Id;
}
