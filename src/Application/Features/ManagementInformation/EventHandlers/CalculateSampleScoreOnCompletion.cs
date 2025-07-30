using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class CalculateSampleScoreOnCompletion(IUnitOfWork unitOfWork)
    : INotificationHandler<OutcomeQualityDipSampleParticipantScoredDomainEvent>
{
    public async Task Handle(OutcomeQualityDipSampleParticipantScoredDomainEvent notification, CancellationToken cancellationToken)
    {
        var db = unitOfWork.DbContext;

        var query = from dipSample in db.OutcomeQualityDipSamples
                    where dipSample.Id == notification.DipSampleId
                    select new
                    {
                        SampleSize = dipSample.Size,
                        CompletedCount = db.OutcomeQualityDipSampleParticipants
                            .Count(dsp => dsp.DipSampleId == dipSample.Id && dsp.CsoReviewedOn.HasValue)
                            + 1 // Include the current completion
                    };

        var result = await query.SingleAsync(cancellationToken);

        if(result.SampleSize == result.CompletedCount)
        {
            var sample = await db.OutcomeQualityDipSamples.SingleAsync(ds => ds.Id == notification.DipSampleId, cancellationToken);
            sample.Complete(notification.ReviewBy, result.CompletedCount);
        }

    }
}
