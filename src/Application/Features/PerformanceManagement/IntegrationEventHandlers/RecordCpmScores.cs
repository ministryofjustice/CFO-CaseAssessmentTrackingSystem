using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class RecordCpmScores(IUnitOfWork unitOfWork) : IIntegrationEventHandler<OutcomeQualityDipSampleVerifyingIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleVerifyingIntegrationEvent message, CancellationToken cancellationToken)
    {
        var db = unitOfWork.DbContext;

        var dipSample = await db.OutcomeQualityDipSamples
            .Include(ds => ds.Participants)
            .FirstAsync(s => s.Id == message.DipSampleId);

        dipSample.Verified(message.UserId);
        await unitOfWork.SaveChangesAsync();
    }
}
