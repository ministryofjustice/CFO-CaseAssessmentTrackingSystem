using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Components.Participants;

public partial class ParticipantDetailsCardComponent
{
    protected override IRequest<Result<GetParticipantDetails.ParticipantDetails>> CreateQuery()
        => new GetParticipantDetails.Query()
        {
            CurrentUser = CurrentUser,
            ParticipantId = ParticipantId
        };
}
