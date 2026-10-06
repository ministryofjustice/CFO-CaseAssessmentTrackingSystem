using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordUsageConsumer(IUnitOfWork unitOfWork)
    : IHandleMessages<UsageTrackedIntegrationEvent>
{
    public async Task Handle(UsageTrackedIntegrationEvent message)
    {
        var usageEvent = UsageEvent.Create(
            area: message.Area,
            activity: message.Activity,
            userId: message.UserId,
            userName: message.UserName,
            tenantId: message.TenantId,
            context: message.Context,
            occurredOn: message.OccurredOn);

        unitOfWork.DbContext.UsageEvents.Add(usageEvent);

        await unitOfWork.SaveChangesAsync();
    }
}
