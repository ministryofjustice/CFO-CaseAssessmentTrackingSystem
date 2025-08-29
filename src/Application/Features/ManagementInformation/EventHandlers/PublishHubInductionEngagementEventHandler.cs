using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishHubInductionEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) 
    : INotificationHandler<HubInductionCreatedDomainEvent>
{
    public async Task Handle(HubInductionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var query = from l in unitOfWork.DbContext.Locations
                    join c in unitOfWork.DbContext.Contracts on l.Contract.Id equals c.Id
                    where l.Id == notification.Item.LocationId
                    select new { l.Name, Contract = c.Description };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var location = await query.SingleAsync(cancellationToken);

        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: notification.Item.ParticipantId,
            Description: $"Took place at {location.Name} by {currentUserService.DisplayName}",
            Category: "Hub Induction",
            EngagedOn: DateOnly.FromDateTime(notification.Item.InductionDate),
            EngagedAtLocation: location.Name,
            EngagedAtContract: location.Contract,
            EngagedWith: currentUserService.DisplayName!,
            EngagedWithTenant: currentUserService.TenantName!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
