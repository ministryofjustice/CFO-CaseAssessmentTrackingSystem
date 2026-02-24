using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class PathwayPlanReviewHistory
{
    protected override IRequest<Result<PathwayPlanReviewHistoryDto[]>> CreateQuery()
        => new GetPathwayPlanReviewHistoryHistory.Query()
        {
            CurrentUser = CurrentUser,
            ParticipantId = ParticipantId
        };
}