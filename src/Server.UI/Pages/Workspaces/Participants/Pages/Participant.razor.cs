using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Models;
using Cfo.Cats.Server.UI.Pages.Participants.Components;
using Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class Participant
{
    [Parameter] public string Id { get; set; } = null!;

    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    private ParticipantCascadingDetails? ParticipantCascadingDetails { get; set; }

    private bool _pathwaySummaryView = false;

    private CasePathwayPlan? _casePathwayPlan;
    private OutcomeQualityDipSamplePathwayPlanComponent? _summaryObjectives;
    private PathwayPlanReviewHistory? _summaryReviewHistory;
    private readonly string _rightToWorkAlertMessage = ConstantString.RightToWorkIsRequiredMessage;

    private readonly string _notActiveInFeedAlertMessage = ConstantString.LicenceEndedWarning;

    private async Task ExpandAllPathwayPlan()
    {
        if (_pathwaySummaryView)
        {
            if (_summaryObjectives is not null)
            {
                await _summaryObjectives.ExpandAll();
            }

            if (_summaryReviewHistory is not null)
            {
                await _summaryReviewHistory.ExpandAll();
            }
        }
        else if (_casePathwayPlan is not null)
        {
            await _casePathwayPlan.ExpandAll();
        }
    }

    private async Task CollapseAllPathwayPlan()
    {
        if (_pathwaySummaryView)
        {
            if (_summaryObjectives is not null)
            {
                await _summaryObjectives.CollapseAll();
            }

            if (_summaryReviewHistory is not null)
            {
                await _summaryReviewHistory.CollapseAll();
            }
        }
        else if (_casePathwayPlan is not null)
        {
            await _casePathwayPlan.CollapseAll();
        }
    }

    private bool ShowRightToWorkWarning() => Data!.IsRightToWorkRequired
                                  && Data!.ConsentStatus == ConsentStatus.GrantedStatus
                                  && Data.HasActiveRightToWork == false;

    protected override void OnDataLoaded(ParticipantSummaryDto data)
    {
        base.OnDataLoaded(data);
        ParticipantCascadingDetails = new()
        {
            Id = data.Id,
            FullName = data.ParticipantName,
            IsActive = data.IsActive,
            ConsentStatus = data.ConsentStatus,
            DateOfFirstConsent = data.DateOfFirstConsent
        };
    }

    protected override IQuery<Result<ParticipantSummaryDto>> CreateQuery() => 
        new GetParticipantSummary.Query()
        {
            ParticipantId = Id,
            CurrentUser = UserProfile
        };
}