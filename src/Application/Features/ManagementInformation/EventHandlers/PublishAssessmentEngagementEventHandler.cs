using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishAssessmentEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : INotificationHandler<AssessmentScoredDomainEvent>
{
    public async Task Handle(AssessmentScoredDomainEvent notification, CancellationToken cancellationToken)
    {
        var location = await unitOfWork.DbContext.Locations
            .Where(l => l.Id == notification.Entity.LocationId)
            .Select(l => l.Name)
            .SingleAsync(cancellationToken);

        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Entity.ParticipantId,
            Description: $"Completed at {location}",
            Category: "Assessment",
            EngagedOn: DateOnly.FromDateTime(notification.Entity.Completed!.Value),
            UserId: currentUserService.UserId!,
            TenantId: currentUserService.TenantId!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
