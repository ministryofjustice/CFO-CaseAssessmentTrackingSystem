using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;
using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ParticipantLabels.IntegrationEventHandlers;

public class RecordHomelessLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordHomelessLabelStatusConsumer> logger)
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
                    new HousingPathway(),
                ]
            }.WithAnswers(participantAssessment.Answers.ToLookup(a => a.QuestionCode, a => a.Answer));

            var pathway = assessment.Pathways.OfType<HousingPathway>().First();

            bool isHomeless = pathway.B1.Answer != B1.HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian;

            ParticipantId participantId = new(assessment.ParticipantId);
            LabelId homelessLabelId = new(new Guid("50176c48-e1f5-48b5-b422-182ef7b54b73"));

            int count = counter.CountOpenLabels(participantId, homelessLabelId);

            if (!isHomeless && count > 0)
            {
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                foreach (var l in labels)
                {
                    if(l.Label.Id == homelessLabelId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantAssessment.LastModifiedBy);   
                    }
                }
            }
            else if (isHomeless && count == 0)
            {
                var homelessLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == homelessLabelId);

                var label = ParticipantLabel.Create(participantId,
                    homelessLabel,
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