using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishWingInductionEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : INotificationHandler<WingInductionCreatedDomainEvent>
{
    public async Task Handle(WingInductionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var location = await unitOfWork.DbContext.Locations
            .Include(l => l.Contract)
            .SingleAsync(l => l.Id == notification.Item.LocationId, cancellationToken);

        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Item.ParticipantId,
            Description: $"Took place at {location.Name} by {currentUserService.DisplayName}",
            Category: "Wing Induction",
            EngagedOn: DateOnly.FromDateTime(notification.Item.InductionDate),
            EngagedAtLocation: location.Name,
            EngagedAtLocationType: location.LocationType.Name,
            EngagedAtContract: location.Contract!.Description,
            EngagedWith: currentUserService.DisplayName!,
            EngagedWithTenant: currentUserService.TenantName!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
