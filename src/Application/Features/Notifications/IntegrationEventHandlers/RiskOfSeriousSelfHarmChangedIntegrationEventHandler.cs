using Cfo.Cats.Application.Features.HmppsExternalApi.IntegrationEvents;
using Cfo.Cats.Domain.ExternalRisk;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Notifications.IntegrationEventHandlers;

public class RiskOfSeriousSelfHarmChangedIntegrationEventHandler(IUnitOfWork unitOfWork, ILogger<RiskOfSeriousSelfHarmChangedIntegrationEventHandler> logger)
    : IHandleMessages<RiskOfSeriousHarmChangedIntegrationEvent>
{
    public async Task Handle(RiskOfSeriousHarmChangedIntegrationEvent message)
    {
        var query = from p in unitOfWork.DbContext.Participants
                            from ei in p.ExternalIdentifiers
                            where ei.Value == message.HmppsId
                            select p.Id;
        
        var participantId = await query.FirstOrDefaultAsync();

        if(participantId is null)
        {
            logger.LogInformation("No participant found for HmppsId {HmppsId}", message.HmppsId);
        }
        else
        {
            logger.LogDebug("Writing risk update for {participantId} - {hmppsId}", participantId, message.HmppsId);

            var update = ParticipantRiskUpdate.Create(participantId, message.HmppsId, message.EventType, message.CreatedDate);
            unitOfWork.DbContext.ParticipantRiskUpdates.Add(update);
            await unitOfWork.SaveChangesAsync();
        }

    }
}