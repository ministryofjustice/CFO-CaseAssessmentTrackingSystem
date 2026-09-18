using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ParticipantLabels.IntegrationEventHandlers;

public class RecordDisabledLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordDisabledLabelStatusConsumer> logger)
    : IHandleMessages<AssessmentScoredIntegrationEvent>
{
    public async Task Handle(AssessmentScoredIntegrationEvent context)
    {
        try
        {
            var participantAssessment = await unitOfWork.DbContext.ParticipantAssessments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id == context.Id);

            if (participantAssessment is null)
            {
                logger.LogWarning("Cannot find assessment {assessmentId}", context.Id);
                return;
            }

            var assessment = new Assessment
            {
                Id = participantAssessment.Id,
                ParticipantId = participantAssessment.ParticipantId,
                Pathways =
                [
                    new HealthAndAddictionPathway(),
                ]
            }.WithAnswers(participantAssessment.Answers.ToLookup(a => a.QuestionCode, a => a.Answer));

            var pathway = assessment.Pathways.OfType<HealthAndAddictionPathway>().First();

            bool isDisabled = pathway.E2.Answer != E2.No;

            ParticipantId participantId = new(assessment.ParticipantId);
            LabelId disabledLabelId = new(new Guid("c4d54b01-c4a2-4368-a806-63307b514c18"));

            int count = counter.CountOpenLabels(participantId, disabledLabelId);

            if (!isDisabled && count > 0)
            {
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                foreach (var l in labels)
                {
                    if(l.Label.Id == disabledLabelId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantAssessment.LastModifiedBy);   
                    }
                }
            }
            else if (isDisabled && count == 0)
            {
                var disabledLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == disabledLabelId);

                var label = ParticipantLabel.Create(participantId,
                    disabledLabel,
                    counter,
                    participantAssessment.LastModifiedBy
                );

                await participantLabelRepository.AddAsync(label);
            }

            await unitOfWork.CommitTransactionAsync();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error adding label for assessment {AssessmentId}", context.Id);
        }
    }
}