using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;
public partial class RiskHistory
{
    protected override IRequest<Result<RiskHistoryDto[]>> CreateQuery()
        => new GetParticipantRiskHistory.Query()
        {
            CurrentUser = CurrentUser,
            ParticipantId = ParticipantId
        };
}