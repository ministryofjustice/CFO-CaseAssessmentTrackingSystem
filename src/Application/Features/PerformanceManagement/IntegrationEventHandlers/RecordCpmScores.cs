using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class RecordCpmScores(IUnitOfWork unitOfWork) : IHandleMessages<OutcomeQualityDipSampleVerifyingIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleVerifyingIntegrationEvent message)
    {
        var db = unitOfWork.DbContext;

        var dipSample = await db.OutcomeQualityDipSamples
            .Include(ds => ds.Participants)
            .FirstAsync(s => s.Id == message.DipSampleId);

        dipSample.Verified(message.UserId);
        await unitOfWork.SaveChangesAsync();
    }
}
