using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class RiskInformationAddedEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<RiskInformationAddedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Participant;

    protected override string GetLine1(RiskInformationAddedDomainEvent notification) => "Risk information added";

    protected override string GetParticipantId(RiskInformationAddedDomainEvent notification) => notification.Item.ParticipantId;
}
