
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantCreatedEventHandler(ICurrentUserService currentUserService, IApplicationDbContext context) : TimelineNotificationHandler<ParticipantCreatedDomainEvent>(currentUserService, context)
{
    protected override string GetLine1(ParticipantCreatedDomainEvent notification) => "Participant record created";
    protected override TimelineEventType GetEventType() => TimelineEventType.Participant;
    protected override string GetParticipantId(ParticipantCreatedDomainEvent notification) => notification.Item.Id;
}
