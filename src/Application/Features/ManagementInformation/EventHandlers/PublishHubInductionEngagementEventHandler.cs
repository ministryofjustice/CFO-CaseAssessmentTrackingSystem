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
            .Where(l => l.Id == notification.Item.LocationId)
            .Select(l => l.Name)
            .SingleAsync(cancellationToken);

        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Item.ParticipantId,
            Description: $"Took place at {location}",
            Category: "Hub Induction",
            EngagedOn: DateOnly.FromDateTime(notification.Item.InductionDate),
            UserId: currentUserService.UserId!,
            TenantId: currentUserService.TenantId!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
