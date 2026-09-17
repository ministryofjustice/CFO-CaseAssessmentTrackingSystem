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

public class RecordGangMemberLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordGangMemberLabelStatusConsumer> logger)
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

            bool gangMamber = pathway.A10.Answer == A10.Yes;

            ParticipantId participantId = new(bio.ParticipantId);
            LabelId gangMemberId = new(new Guid("f182441a-905f-4846-ad5e-f2acffe86e9a"));

            int count = counter.CountOpenLabels(participantId, gangMemberId);

            if (gangMamber == false && count > 0)
            {
                // Not a gang member. Close any open Gang Member labels
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                // there should be only one, but just in case
                foreach (var l in labels)
                {
                    if(l.Label.Id == gangMemberId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantBio.LastModifiedBy);   
                    }
                }
            }
            else if (gangMamber && count == 0)
            {
                var gangMemberLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == gangMemberId);

                var label = ParticipantLabel.Create(participantId,
                    gangMemberLabel,
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
