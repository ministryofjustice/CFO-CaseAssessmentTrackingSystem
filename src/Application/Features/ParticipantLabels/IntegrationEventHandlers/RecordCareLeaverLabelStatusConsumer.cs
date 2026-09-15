using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.RecentExperiences;
using Cfo.Cats.Application.Features.Bios.IntegrationEvents;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ParticipantLabels.IntegrationEventHandlers;

public class RecordCareLeaverLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordCareLeaverLabelStatusConsumer> logger)
    : IHandleMessages<BioSubmittedIntegrationEvent>
{
    public async Task Handle(BioSubmittedIntegrationEvent context)
    {
        try
        {
            var participantBio = await unitOfWork.DbContext.ParticipantBios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id == context.BioId);

            if (participantBio is null)
            {
                logger.LogWarning("Cannot find bio {bioId}", context.BioId);
                return;
            }

            var bio = new Bio
            {
                Id = participantBio.Id,
                ParticipantId = participantBio.ParticipantId,
                Pathways =
                [
                    new DiversityPathway(),
                    new ChildhoodExperiencesPathway(),
                    new RecentExperiencesPathway(),
                ]
            }.WithAnswers(participantBio.Answers.ToLookup(a => a.QuestionCode, a => a.Answer));

            var pathway = bio.Pathways.OfType<ChildhoodExperiencesPathway>().First();

            bool careLeaver = pathway.B1.Answer == B1.Yes;

            ParticipantId participantId = new(bio.ParticipantId);
            LabelId careLeaverId = new(new Guid("a8e17308-74f6-4f7c-9a1a-c50b5cc85b73"));

            int count = counter.CountOpenLabels(participantId, careLeaverId);

            if (careLeaver == false && count > 0)
            {
                // Not a care leaver. Close any open Care Leaver labels
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                // there should be only one, but just in case
                foreach (var l in labels)
                {
                    if(l.Label.Id == careLeaverId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantBio.LastModifiedBy);   
                    }
                }
            }
            else if (careLeaver && count == 0)
            {
                var careLeaverLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == careLeaverId);

                var label = ParticipantLabel.Create(participantId,
                    careLeaverLabel,
                    counter,
                    participantBio.LastModifiedBy
                );

                await participantLabelRepository.AddAsync(label);
            }

            await unitOfWork.CommitTransactionAsync();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error adding label for bio {BioId}", context.BioId);
        }
    }
}
