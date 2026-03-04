using Cfo.Cats.Application.Features.Bios.DTOs;
using Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;
using Cfo.Cats.Application.Features.Bios.IntegrationEvents;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ParticipantLabels.IntegrationEventHandlers;

public class RecordVeteranLabelStatusConsumer(IUnitOfWork unitOfWork, IParticipantLabelsCounter counter, IParticipantLabelRepository participantLabelRepository, ILogger<RecordVeteranLabelStatusConsumer> logger)
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

            var bio = JsonConvert.DeserializeObject<Bio>(participantBio.BioJson, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;

            var pathway = bio.Pathways.OfType<DiversityPathway>().First();

            bool allNo = (pathway.A7.Answer, pathway.A8.Answer, pathway.A9.Answer) switch
            {
                (A7.Yes, _, _) => false,
                (_, A8.Yes, _) => false,
                (_, _, A9.Yes) => false,
                _ => true
            };

            ParticipantId participantId = new(bio.ParticipantId);
            LabelId veteranId = new(new Guid("a154163b-63c8-43b1-887f-9853848c58f6"));

            int count = counter.CountOpenLabels(participantId, veteranId);

            if (allNo && count > 0)
            {
                // all are no. Close any open Veteran labels
                var labels = await participantLabelRepository.GetByParticipantIdAsync(participantId);

                // there should be only one, but just in case
                foreach (var l in labels)
                {
                    if(l.Label.Id == veteranId && l.Lifetime.EndDate >= DateTime.UtcNow)
                    {
                        l.Close(force: true, closedBy: participantBio.LastModifiedBy);   
                    }
                }
            }
            else if (allNo == false && count == 0)
            {
                var veteranLabel = await unitOfWork.DbContext.Labels
                                .SingleAsync(l => l.Id == veteranId);

                var label = ParticipantLabel.Create(participantId,
                    veteranLabel,
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
