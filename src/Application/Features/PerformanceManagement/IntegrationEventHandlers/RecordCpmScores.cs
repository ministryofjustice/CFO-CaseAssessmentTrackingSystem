using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class RecordCpmScores(IUnitOfWork unitOfWork) : IHandleMessages<OutcomeQualityDipSampleVerifyingIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleVerifyingIntegrationEvent message)
    {
        var db = unitOfWork.DbContext;

        var dipSample = await db.OutcomeQualityDipSamples
            .FirstAsync(s => s.Id == message.DipSampleId);

        var participants = await db.OutcomeQualityDipSampleParticipants
            .Where(s => s.DipSampleId == message.DipSampleId)
            .ToArrayAsync();

        foreach(var participant in participants)
        {
            if (participant.CpmIsCompliant.IsAnswer == false)
            {
                var cpmAnswer = participant.CsoIsCompliant switch
                {
                    var c when c.Name == ComplianceAnswer.Compliant.Name => ComplianceAnswer.AutoCompliant,
                    var c when c.Name == ComplianceAnswer.NotCompliant.Name => ComplianceAnswer.AutoNotCompliant,
                    _ => throw new ApplicationException("Invalid answer")
                };

                participant.CpmAnswer(
                    cpmAnswer,
                    participant.CsoComments!,
                    message.UserId,
                    message.OccurredOn
                );
            }

            if (participant.CsoIsCompliant.IsAccepted == participant.CpmIsCompliant.IsAccepted)
            {
                participant.FinalAnswer(participant.CpmIsCompliant, participant.CpmComments!, participant.CpmReviewedBy!,
                    message.OccurredOn);
            }
        }

        dipSample.Verified(participants.Count(c => c.CpmIsCompliant.IsAccepted));

        await unitOfWork.SaveChangesAsync();
    }
}
