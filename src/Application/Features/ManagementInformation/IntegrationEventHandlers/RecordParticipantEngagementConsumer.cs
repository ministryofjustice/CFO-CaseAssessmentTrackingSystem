using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordParticipantEngagementConsumer(IUnitOfWork unitOfWork)
    : IHandleMessages<ParticipantEngagedIntegrationEvent>
{
    public async Task Handle(ParticipantEngagedIntegrationEvent message)
    {
        var engagement = new ParticipantEngagement() 
        { 
            ParticipantId = message.ParticipantId,
            Description = message.Description,
            Category = message.Category,
            EngagedOn = message.EngagedOn,
            EngagedAtLocation = message.EngagedAtLocation,
            EngagedAtContract = message.EngagedAtContract,
            EngagedWith = message.EngagedWith,
            EngagedWithTenant = message.EngagedWithTenant
        };

        unitOfWork.DbContext.ParticipantEngagements.Add(engagement);

        await unitOfWork.SaveChangesAsync();
    }

}
