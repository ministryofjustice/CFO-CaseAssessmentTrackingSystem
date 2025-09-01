using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordParticipantEngagementConsumer(IUnitOfWork unitOfWork)
    : IHandleMessages<ParticipantEngagedIntegrationEvent>
{
    public async Task Handle(ParticipantEngagedIntegrationEvent message)
    {
        var engagement = ParticipantEngagement.Create();

        engagement.ParticipantId = message.ParticipantId;
        engagement.Description = message.Description;
        engagement.Category = message.Category;
        engagement.EngagedOn = message.EngagedOn;
        engagement.EngagedAtLocation = message.EngagedAtLocation;
        engagement.EngagedAtContract = message.EngagedAtContract;
        engagement.EngagedWith = message.EngagedWith;
        engagement.EngagedWithTenant = message.EngagedWithTenant;

        unitOfWork.DbContext.ParticipantEngagements.Add(engagement);

        await unitOfWork.SaveChangesAsync();
    }

}
