using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Infrastructure.Persistence.Repositories;

public class ParticipantLabelRepository(IUnitOfWork unitOfWork) : IParticipantLabelRepository
{
    public async Task AddAsync(ParticipantLabel participantLabel)
        => await unitOfWork.DbContext.ParticipantLabels.AddAsync(participantLabel);

    public async Task<ParticipantLabel> GetByIdAsync(ParticipantLabelId participantLabelId) => 
        await unitOfWork.DbContext.ParticipantLabels
                .Include(x => x.Label)
                .FirstOrDefaultAsync(p => p.Id == participantLabelId)
                ?? throw new NotFoundException("ParticipantLabel", participantLabelId);
    public async Task<ParticipantLabel[]> GetByParticipantIdAsync(ParticipantId participantId) => 
        await unitOfWork.DbContext.ParticipantLabels
                .Include(x => x.Label)
                .Where(pl => EF.Property<string>(pl, "_participantId") == participantId.Value)
                .ToArrayAsync();
}