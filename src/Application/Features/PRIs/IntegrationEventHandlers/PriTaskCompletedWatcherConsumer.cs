using Cfo.Cats.Application.Features.PathwayPlans.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using MassTransit;

namespace Cfo.Cats.Application.Features.PRIs.IntegrationEventHandlers;

public class PriTaskCompletedWatcherConsumer(IUnitOfWork unitOfWork, ILogger<PriTaskCompletedWatcherConsumer> logger) : IConsumer<ObjectiveTaskCompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ObjectiveTaskCompletedIntegrationEvent> context)
    {
        if (context.Message.IsMandatoryTask == false)
        {
            logger.LogDebug("Ignoring non mandatory task");
            return;
        }

        if (context.Message.Index == 2)
        {
            logger.LogDebug("Ignoring mandatory task that is not index 2");
            return;
        }

        if (context.Message.CompletionState == CompletionStatus.NotRequired.Name)
        {
            logger.LogDebug("Ignoring mandatory task that has been completed as not required");
            return;
        }

        var query = from p in unitOfWork.DbContext.PRIs
            where p.ObjectiveId == context.Message.ObjectiveId
                && p.ActualReleaseDate != null
            select new
            {
                PRIId = p.Id,
                ActualReleaseDate = p.ActualReleaseDate!
            };

        var result = await query.FirstOrDefaultAsync();

        // check if we are a PRI objective
        if (result is not null)
        {
            // we are a PRI objective, and we have a release date. Ergo, raise an event
            await context.Publish(new PRIThroughTheGateCompletedIntegrationEvent(result.PRIId));
        }

    }
}