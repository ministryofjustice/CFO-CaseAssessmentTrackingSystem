using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class RecordCsoScores(IUnitOfWork unitOfWork) : IHandleMessages<OutcomeQualityDipSampleCompletedIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleCompletedIntegrationEvent message)
    {
        var db = unitOfWork.DbContext;

        var dipSample = await db.OutcomeQualityDipSamples.FirstAsync(s => s.Id == message.DipSampleId);
        var participants = await db.OutcomeQualityDipSampleParticipants
                .Where(s => s.DipSampleId == message.DipSampleId)
                .Select(s => s.CsoIsCompliant)
                .ToArrayAsync();

        if (participants.All(a => a.IsAnswer))
        {
            dipSample.Review(message.ReviewBy, participants.Count(p => p.IsAccepted));
            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
    }
}