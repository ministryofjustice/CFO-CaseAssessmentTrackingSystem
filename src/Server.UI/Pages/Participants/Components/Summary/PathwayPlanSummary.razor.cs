using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Participants.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class PathwayPlanSummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private bool HasPathwayPlan => ParticipantSummaryDto.PathwayPlan is not null;

    private bool HasPathwayBeenReviewed => HasPathwayPlan && ParticipantSummaryDto.PathwayPlan?.LastReviewed is not null;
}
