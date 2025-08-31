using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Components.Participants;

/// <summary>
/// This component displays key header information inside of a mud card.
/// </summary>
public partial class ParticipantDetailsCardComponent
{
    protected override IRequest<Result<ParticipantHeaderDetailsDto>> CreateQuery()
        => new GetParticipantDetails.Query()
        {
            CurrentUser = CurrentUser,
            ParticipantId = ParticipantId
        };
}
