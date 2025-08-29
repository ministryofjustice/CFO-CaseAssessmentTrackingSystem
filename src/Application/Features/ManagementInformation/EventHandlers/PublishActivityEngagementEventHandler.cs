using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishActivityEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : INotificationHandler<ActivityCreatedDomainEvent>
{
    public async Task Handle(ActivityCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Entity.ParticipantId,
            Description: $"{notification.Entity.Category.Name} at {notification.Entity.TookPlaceAtLocation.Name} by {currentUserService.DisplayName}",
            Category: notification.Entity.Definition.Type.Name,
            EngagedOn: DateOnly.FromDateTime(notification.Entity.CommencedOn),
            EngagedAtLocation: notification.Entity.TookPlaceAtLocation.Name,
            EngagedAtContract: notification.Entity.TookPlaceAtContract.Description,
            EngagedWith: currentUserService.DisplayName!,
            EngagedWithTenant: currentUserService.TenantName!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
