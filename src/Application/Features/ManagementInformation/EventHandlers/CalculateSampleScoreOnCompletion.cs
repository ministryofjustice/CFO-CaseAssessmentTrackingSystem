using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class CalculateSampleScoreOnCompletion(IUnitOfWork unitOfWork)
    : INotificationHandler<OutcomeQualityDipSampleParticipantScoredDomainEvent>
{
    public async Task Handle(OutcomeQualityDipSampleParticipantScoredDomainEvent notification, CancellationToken cancellationToken)
    {
        var db = unitOfWork.DbContext;

        var query = db.OutcomeQualityDipSampleParticipants
            .AsNoTracking()
            .Where(p => p.DipSampleId == notification.DipSampleId)
            .GroupBy(p => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Completed = g.Count(p => p.CsoReviewedOn != null)
            });

        var result = await query.SingleAsync(cancellationToken);

        if((result.Completed + 1) >= result.Total)
        {
            var sample = await db.OutcomeQualityDipSamples.SingleAsync(ds => ds.Id == notification.DipSampleId, cancellationToken);
            var all = await db.OutcomeQualityDipSampleParticipants
                .Where(ds => ds.DipSampleId == notification.DipSampleId)
                .Select(s => s.CsoIsCompliant).ToArrayAsync(cancellationToken);
            
            sample.Complete(notification.ReviewBy, all.Count( s => s.IsAccepted ));
        }

    }
}
