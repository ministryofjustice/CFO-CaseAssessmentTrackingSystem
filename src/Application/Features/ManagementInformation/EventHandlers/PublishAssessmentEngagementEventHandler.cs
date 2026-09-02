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
            .Include(l => l.Contract)
            .SingleAsync(l => l.Id == notification.Entity.LocationId, cancellationToken);

        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Entity.ParticipantId,
            Description: $"Completed at {location.Name} by {currentUserService.DisplayName}",
            Category: "Assessment",
            EngagedOn: DateOnly.FromDateTime(notification.Entity.Completed!.Value),
            EngagedAtLocation: location.Name,
            EngagedAtLocationType: location.LocationType.Name,
            EngagedAtContract: location.Contract!.Description,
            EngagedWith: currentUserService.DisplayName!,
            EngagedWithTenant: currentUserService.TenantName!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
