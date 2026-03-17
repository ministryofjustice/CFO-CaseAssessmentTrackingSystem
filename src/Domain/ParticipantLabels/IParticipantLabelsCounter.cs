using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Domain.ParticipantLabels;

public interface IParticipantLabelsCounter
{
    int CountOpenLabels(ParticipantId participantId, LabelId labelId);
}