using Cfo.Cats.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class RiskInformationCompletedHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<RiskInformationCompletedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Participant;

    protected override string GetLine1(RiskInformationCompletedDomainEvent notification) => "Risk information completed";

    protected override string GetParticipantId(RiskInformationCompletedDomainEvent notification) => notification.Item.ParticipantId;
}
