using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishRiskEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : 
    INotificationHandler<RiskInformationCompletedDomainEvent>, 
    INotificationHandler<RiskInformationReviewedDomainEvent>    
{
    public async Task Handle(RiskInformationCompletedDomainEvent notification, CancellationToken cancellationToken)
        => await Handle(notification.Item, notification.DateOccurred.DateTime, "added");

    public async Task Handle(RiskInformationReviewedDomainEvent notification, CancellationToken cancellationToken)
        => await Handle(notification.Item, notification.DateOccurred.DateTime, "reviewed");

    private async Task Handle(Risk risk, DateTime engagedOn, string engagementType)
    {
        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: risk.ParticipantId,
            Description: $"Risk {engagementType}: {risk.ReviewReason.Name}",
            Category: "Risk",
            EngagedOn: DateOnly.FromDateTime(engagedOn),
            UserId: currentUserService.UserId!,
            TenantId: currentUserService.TenantId!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
