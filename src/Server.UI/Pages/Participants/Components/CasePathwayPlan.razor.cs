using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Server.UI.Models;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class CasePathwayPlan
{
    [CascadingParameter(Name = "ParticipantDetails")]
    public ParticipantCascadingDetails? ParticipantDetails { get; set; }

    private ViewPathwayPlan? _viewPathwayPlan;
    private PathwayPlanReviewHistory? _reviewHistory;

    public async Task ExpandAll()
    {
        _viewPathwayPlan?.ExpandAll();
        if (_reviewHistory is not null)
        {
            await _reviewHistory.ExpandAll();
        }
    }

    public async Task CollapseAll()
    {
        _viewPathwayPlan?.CollapseAll();
        if (_reviewHistory is not null)
        {
            await _reviewHistory.CollapseAll();
        }
    }
}