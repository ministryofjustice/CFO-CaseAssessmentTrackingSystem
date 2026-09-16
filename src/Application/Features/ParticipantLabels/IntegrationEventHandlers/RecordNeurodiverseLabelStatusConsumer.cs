using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ParticipantLabels.IntegrationEventHandlers;

public class RecordNeurodiverseLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordNeurodiverseLabelStatusConsumer> logger)
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
                    new EducationPathway(),
                ]
            }.WithAnswers(participantAssessment.Answers.ToLookup(a => a.QuestionCode, a => a.Answer));

            var pathway = assessment.Pathways.OfType<EducationPathway>().First();

            bool isNeurodiverse = pathway.D3.Answers?.Any(answer => answer != D3.NoneOftheGivenOptions) == true;

            ParticipantId participantId = new(assessment.ParticipantId);
            LabelId neurodiverseLabelId = new(new Guid("499a6380-5f0c-40d5-83c6-0bb1158ccc21"));

            int count = counter.CountOpenLabels(participantId, neurodiverseLabelId);

            if (!isNeurodiverse && count > 0)
            {
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                foreach (var l in labels)
                {
                    if(l.Label.Id == neurodiverseLabelId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantAssessment.LastModifiedBy);   
                    }
                }
            }
            else if (isNeurodiverse && count == 0)
            {
                var neurodiverseLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == neurodiverseLabelId);

                var label = ParticipantLabel.Create(participantId,
                    neurodiverseLabel,
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