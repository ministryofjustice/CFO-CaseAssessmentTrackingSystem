using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Domain.ParticipantLabels.Rules;

public class LabelMustNotAlreadyBeOpen(ParticipantId participantId, LabelId labelId, IParticipantLabelsCounter counter)
    : IBusinessRule
{
    public bool IsBroken()
    {
        var count = counter.CountOpenLabels(participantId, labelId);
        return count > 0;
    }

    public string Message => "Participant labels must be unique";
}