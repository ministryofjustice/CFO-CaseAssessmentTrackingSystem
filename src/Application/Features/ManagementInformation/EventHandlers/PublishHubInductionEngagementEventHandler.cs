using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishHubInductionEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) 
    : INotificationHandler<HubInductionCreatedDomainEvent>
{
    public async Task Handle(HubInductionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var location = await unitOfWork.DbContext.Locations
            .Include(l => l.Contract)
            .SingleAsync(l => l.Id == notification.Item.LocationId, cancellationToken);

        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Item.ParticipantId,
            Description: $"Took place at {location.Name} by {currentUserService.DisplayName}",
            Category: "Hub Induction",
            EngagedOn: DateOnly.FromDateTime(notification.Item.InductionDate),
            EngagedAtLocation: location.Name,
            EngagedAtLocationType: location.LocationType.Name,
            EngagedAtContract: location.Contract!.Description,
            EngagedWith: currentUserService.DisplayName!,
            EngagedWithTenant: currentUserService.TenantName!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
