using Cfo.Cats.Application.Features.PathwayPlans.IntegrationEvents;
using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Rebus.Handlers;


namespace Cfo.Cats.Application.Features.PRIs.IntegrationEventHandlers;

public class PriTaskCompletedWatcherConsumer(IUnitOfWork unitOfWork, ILogger<PriTaskCompletedWatcherConsumer> logger) : IHandleMessages<ObjectiveTaskCompletedIntegrationEvent>
{
    public async Task Handle(ObjectiveTaskCompletedIntegrationEvent context)
    {
        if (context.IsMandatoryTask == false)
        {
            logger.LogDebug("Ignoring non mandatory task");
            return;
        }

        if (context.Index != 2)
        {
            logger.LogDebug("Ignoring mandatory task that is not index 2");
            return;
        }

        if (context.CompletionState == CompletionStatus.NotRequired.Name)
        {
            logger.LogDebug("Ignoring mandatory task that has been completed as not required");
            return;
        }

        var query = from p in unitOfWork.DbContext.PRIs
            where p.ObjectiveId == context.ObjectiveId
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
            await unitOfWork.DbContext.InsertOutboxMessage(
                new PRIThroughTheGateCompletedIntegrationEvent(result
                    .PRIId));
            await unitOfWork.SaveChangesAsync();
        }

    }
}