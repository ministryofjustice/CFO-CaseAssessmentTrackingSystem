using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Application.Features.ParticipantLabels.GetParticipantLabels;

/// <summary>
/// Class that represents selected labels for a contract
/// </summary>
public class GetParticipantLabelsDto(ParticipantId participantId)
{
    public ParticipantId ParticipantId { get; } = participantId;

    public required ParticipantLabelDto[] Labels { get; set;}
}
