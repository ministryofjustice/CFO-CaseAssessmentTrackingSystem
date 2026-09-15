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

public class RecordTransgenderLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordTransgenderLabelStatusConsumer> logger)
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

            var pathway = bio.Pathways.OfType<DiversityPathway>().First();

            bool transgender = pathway.A4.Answer == A4.Yes;

            ParticipantId participantId = new(bio.ParticipantId);
            LabelId transgenderId = new(new Guid("c4962b22-8897-4771-b80f-334eb98b6920"));

            int count = counter.CountOpenLabels(participantId, transgenderId);

            if (transgender == false && count > 0)
            {
                // Not transgender. Close any open Transgender labels
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                // there should be only one, but just in case
                foreach (var l in labels)
                {
                    if(l.Label.Id == transgenderId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantBio.LastModifiedBy);   
                    }
                }
            }
            else if (transgender && count == 0)
            {
                var transgenderLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == transgenderId);

                var label = ParticipantLabel.Create(participantId,
                    transgenderLabel,
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
