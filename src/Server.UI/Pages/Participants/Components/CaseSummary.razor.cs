using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Assessments.Commands;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Risk;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseSummary
{
    private AssessmentSummaryDto? _latestAssessment;

    [Inject] 
    private IUserService UserService { get; set; } = default!;

    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _latestAssessment = ParticipantSummaryDto.Assessments is []
            ? null
            : ParticipantSummaryDto.Assessments.OrderByDescending(a => a.AssessmentDate)
                .First();
    }

    public async Task BeginAssessment()
    {
        var command = new BeginAssessment.Command
        {
            ParticipantId = ParticipantSummaryDto.Id
        };
        var result = await GetNewMediator().Send(command);

        if (result.Succeeded)
        {
            Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/assessment/{result.Data}");
        }
    }
    
    public void ContinueAssessment()
    {
        Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/assessment/{_latestAssessment!.AssessmentId}");
    }
    
    /// <summary>
    /// If true, indicates we are creating our first ever assessment. 
    /// </summary>
    private bool CanBeginAssessment() => _latestAssessment == null;
    
    /// <summary>
    /// If true indicates we have an assessment that is continuable
    /// (i.e. not scored)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private bool CanContinueAssessment()
    {
        return _latestAssessment is
        {
            AssessmentScored: false
        } ;
    }
    
    /// <summary>
    /// If true indicates we have an assessment that is recreatable
    /// (i.e. we have a scored assessment and are not in QA)
    /// </summary>
    private bool CanReassess()
    {
        return _latestAssessment is
               {
                   AssessmentScored: true
               } &&
               ParticipantSummaryDto.EnrolmentStatus.StatusSupportsReassessment();

    }

    private bool HasRiskInformation() => ParticipantSummaryDto.LatestRisk is not null;
    private bool CanAddRiskInformation() => HasRiskInformation() is false;
    private bool CanReviewRiskInformation() => HasRiskInformation();

    public async Task ReviewRiskInformation()
    {
        var command = new AddRisk.Command
        {
            ParticipantId = ParticipantSummaryDto.Id
        };

        var parameters = new DialogParameters<ReviewRiskDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = DialogService.Show<ReviewRiskDialog>("Review risk information for a participant", parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            await AddRiskInformation(command);
        }
    }

    public async Task AddRiskInformation(AddRisk.Command? command = null)
    {
        command ??= new AddRisk.Command
        {
            ParticipantId = ParticipantSummaryDto.Id,
            ReviewReason = RiskReviewReason.InitialReview
        };

        var result = await GetNewMediator().Send(command);

        if(result.Succeeded is false)
        {
            return;
        }

        if(command.ReviewReason.RequiresFurtherInformation)
        {
            Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/risk/{result.Data}");
        }
        else
        {
            Navigation.Refresh(true);
        }

    }

}
