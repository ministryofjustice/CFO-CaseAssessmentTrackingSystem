using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class BioSubmittedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<BioSubmittedDomainEvent>(currentUserService, unitOfWork)
{
    protected override string GetLine1(BioSubmittedDomainEvent notification) => "Bio Submitted";
    protected override TimelineEventType GetEventType() => TimelineEventType.Bio;
    protected override string GetParticipantId(BioSubmittedDomainEvent notification)
        => notification.Item.ParticipantId;
}