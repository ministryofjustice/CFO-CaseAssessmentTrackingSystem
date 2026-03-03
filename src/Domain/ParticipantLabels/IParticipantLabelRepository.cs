using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Domain.ParticipantLabels;

public interface IParticipantLabelRepository
{
    Task AddAsync(ParticipantLabel participantLabel);
    Task<ParticipantLabel> GetByIdAsync(ParticipantLabelId participantLabelId);
    Task<ParticipantLabel[]> GetByParticipantIdAsync(ParticipantId participantId);
}