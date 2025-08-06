using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Assessments.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Humanizer;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class AssessmentSummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private string _noAssessmentInfo = String.Empty;
    private string _AssessmentNotCompletedInfo = String.Empty;
    private string _assessmentDueInfo = String.Empty;
    private string _assessmentDueTooltipText = String.Empty;
    private string _assessmentDueIcon = String.Empty;
    private Color _assessmentDueIconColor = Color.Transparent;

    private AssessmentSummaryDto? _latestAssessment;

    protected override void OnParametersSet()
    {
        _latestAssessment = ParticipantSummaryDto.Assessments is []
            ? null
            : ParticipantSummaryDto.Assessments.OrderByDescending(a => a.AssessmentDate)
                .First();

        if (_latestAssessment is null)
        {
            _noAssessmentInfo = "No assessment has been created.";
        }
        else if (_latestAssessment.Completed.HasValue)
        {
            DateOnly _assessmentDueDate = DateOnly.FromDateTime(_latestAssessment.Completed.Value.AddMonths(3));

            _assessmentDueInfo = $"Due {_latestAssessment.Completed.Value.AddMonths(3).Humanize()}";
            _assessmentDueTooltipText = String.Format("New Assessment is due {0}", _assessmentDueDate);

            int _assessmentDueInDays = _assessmentDueDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow.Date).DayNumber;
            switch (_assessmentDueInDays)
            {
                case var _ when _assessmentDueInDays <= 0:
                    _assessmentDueIcon = Icons.Material.Filled.Error;
                    _assessmentDueIconColor = Color.Error;
                    break;
                case var _ when _assessmentDueInDays >= 0 && _assessmentDueInDays <= 14:
                    _assessmentDueIcon = Icons.Material.Filled.Warning;
                    _assessmentDueIconColor = Color.Warning;
                    break;
            }
        }
        else
        {
            _assessmentDueIcon = Icons.Material.Filled.Warning;
            _assessmentDueIconColor = Color.Warning;
            _AssessmentNotCompletedInfo = "Assessment not completed.";
        }

    }
    
    public async Task BeginAssessment()
    {
        var parameters = new DialogParameters<AddAssessmentDialog>()
        {
            { x => x.Model, new BeginAssessment.Command() { ParticipantId = ParticipantSummaryDto.Id } }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await DialogService.ShowAsync<AddAssessmentDialog>("Begin assessment for participant", parameters, options);

        var state = await dialog.Result;

        if (state is { Canceled: false })
        {
            Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/assessment/{state.Data}");
        }
    }

    public void ContinueAssessment()
    {
        if (CanContinueAssessment())
        {
            Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/assessment/{_latestAssessment!.AssessmentId}");
        }
        else
        {
            Snackbar.Add($"Unable to continue Assessment");
        }
    }

    /// <summary>
    /// If true, indicates we are creating our first ever assessment. 
    /// </summary>
    private bool CanBeginAssessment()
    {
        return _latestAssessment == null
            && ParticipantSummaryDto.IsActive;
    }

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
        }
        && ParticipantSummaryDto.IsActive;

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
        }
        && ParticipantSummaryDto.EnrolmentStatus.SupportsReassessment()
        && ParticipantSummaryDto.IsActive;
    }
}
