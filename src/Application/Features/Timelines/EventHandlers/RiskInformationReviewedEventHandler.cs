using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class RiskInformationReviewedEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<RiskInformationReviewedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Participant;

    protected override string GetLine1(RiskInformationReviewedDomainEvent notification) => "Risk information reviewed";

    protected override string GetParticipantId(RiskInformationReviewedDomainEvent notification) => notification.Item.ParticipantId;
}
