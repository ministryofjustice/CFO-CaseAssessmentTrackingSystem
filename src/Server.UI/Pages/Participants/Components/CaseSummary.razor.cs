using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Assessments.Commands;
using Cfo.Cats.Application.Features.Bios.Commands;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Risk;
using Humanizer;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseSummary
{
    private AssessmentSummaryDto? _latestAssessment;
    private BioSummaryDto? _bio;
    private PriSummaryDto? _latestPRI = null;
    [Inject] 
    private IUserService UserService { get; set; } = default!;

    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    private string _riskInfo = String.Empty;
    private string _riskTooltipText = String.Empty;
    private string _riskIcon = String.Empty;
    private Color _riskIconColor = Color.Transparent;

    private bool _showTTGDue;
    private string _noPriInfo = String.Empty;
    private string _priDueInfo = String.Empty;
    private string _priDueTooltipText = String.Empty;
    private string _priDueIcon = String.Empty;
    private Color _priDueIconColor = Color.Transparent;

    private string _noAssessmentInfo = String.Empty;
    private string _AssessmentNotCompletedInfo = String.Empty;
    private string _assessmentDueInfo = String.Empty;
    private string _assessmentDueTooltipText = String.Empty;
    private string _assessmentDueIcon = String.Empty;
    private Color _assessmentDueIconColor = Color.Transparent;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _latestAssessment = ParticipantSummaryDto.Assessments is []
            ? null
            : ParticipantSummaryDto.Assessments.OrderByDescending(a => a.AssessmentDate)
                .First();
        _bio = ParticipantSummaryDto.BioSummary;

        _latestPRI = ParticipantSummaryDto.LatestPri;

        SetRiskDueWarning();
        SetPriDueWarning();
        SetAssessmentDueWarning();
    }

    void SetRiskDueWarning()
    {
        if (ParticipantSummaryDto.RiskDue.HasValue)
        {
            var datePart = ParticipantSummaryDto.RiskDue.Value.Date;

            _riskInfo = datePart.Humanize();
            _riskTooltipText = String.Format("Due {0}", DateOnly.FromDateTime(datePart));

            int _dueInDays = ParticipantSummaryDto.RiskDueInDays!.Value!;
            switch (_dueInDays)
            {
                case var _ when _dueInDays <= 0:
                    _riskIcon = Icons.Material.Filled.Error;
                    _riskIconColor = Color.Error;
                    break;
                case var _ when _dueInDays >= 0 && _dueInDays <= 14:
                    _riskIcon = Icons.Material.Filled.Warning;
                    _riskIconColor = Color.Warning;
                    break;
            }
        }
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
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
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
        };
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
               ParticipantSummaryDto.EnrolmentStatus.SupportsReassessment();
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
        var dialog = await DialogService.ShowAsync<ReviewRiskDialog>("Review risk information for a participant", parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            await AddRiskInformation(command);
        }
    }

    public async Task OnClickAddRiskInformation()
    {
        var command = new AddRisk.Command
        {
            ParticipantId = ParticipantSummaryDto.Id,
            ReviewReason = RiskReviewReason.InitialReview
        };

        var parameters = new DialogParameters<ReviewRiskDialog>()
        {
            { x => x.Model, command },
            { x => x.AddReviewRequest, true}
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await DialogService.ShowAsync<ReviewRiskDialog>("Add risk information for a participant", parameters, options);

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

        if (result.Succeeded is false)
        {
            return;
        }

        if (command.ReviewReason.RequiresFurtherInformation)
        {
            Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/risk/{result.Data}");
        }
        else
        {
            Navigation.Refresh(true);
        }
    }

    public async Task ExpandRiskInformation()
    {
        if (ParticipantSummaryDto.LatestRisk is null)
        {
            return;
        }

        var parameters = new DialogParameters<ExpandedRiskDialog>()
        {
            { x => x.Model, ParticipantSummaryDto.LatestRisk }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<ExpandedRiskDialog>("Risk Summary", parameters, options);

        var result = await dialog.Result;
    }

    public async Task BeginBio()
    {
        var command = new BeginBio.Command
        {
            ParticipantId = ParticipantSummaryDto.Id
        };

        var result = await GetNewMediator().Send(command);

        if (result.Succeeded)
        {
            Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/bio/{result.Data}");
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
    }

    public async Task SkipBioForNow()
    {
        var command = new SkipBioForNow.Command()
        {
            ParticipantId = ParticipantSummaryDto.Id
        };

        var result = await GetNewMediator().Send(command);
        if (result.Succeeded)
        {
            Snackbar.Add($"Bio skipped for now, you can add bio information at any time by clicking Continue Bio button", Severity.Info,
            config => {
                config.ShowTransitionDuration = 500;
                config.HideTransitionDuration = 500;
                config.ShowCloseIcon = false;
            });
            Navigation.Refresh(true);
        }
        else
        {
            Snackbar.Add($"Skipping bio failed", Severity.Error);
        }
    }

    public void ContinueBio()
    {
        Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/bio/{_bio!.BioId}");
    }

    /// <summary>
    /// If true, indicates we are creating Bio. 
    /// </summary>
    private bool CanBeginBio() => _bio == null;

    private bool CanRestartBio() => _bio?.BioStatus == BioStatus.Complete;

    /// <summary>
    /// If true indicates we have a Bio that is continuable
    /// (i.e. Id is not null or do we need a status (Complete or Incomplete etc.))
    /// </summary>
    /// <returns></returns>
    private bool CanContinueBio() => _bio?.BioStatus == BioStatus.InProgress;

    /// <summary>
    /// If true, indicates that either the bio doesn't exist OR No step is completed yet  
    /// </summary>
    private bool CanSkipBio()
    {
        return _bio is null || _bio!.BioStatus == BioStatus.NotStarted;
    }

    private bool HasPathwayPlan => ParticipantSummaryDto.PathwayPlan is not null;
    private bool HasPathwayBeenReviewed => HasPathwayPlan && ParticipantSummaryDto.PathwayPlan?.LastReviewed is not null;

    private bool CanAddPRI() => _latestPRI == null && ParticipantSummaryDto.LocationType.IsCustody && ParticipantSummaryDto.LocationType.IsMapped;
    public void BeginPRI()
    {
        Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/PRI");
    }
    void SetPriDueWarning()
    {
        _priDueIcon = String.Empty;
        _priDueIconColor = Color.Transparent;

        if (_latestPRI is null)
        {
            _noPriInfo = ParticipantSummaryDto.LocationType switch
            {
                { IsCustody: true, IsMapped: true } => "No PRI has been created.",
                { IsCommunity: true } => "Not available in the community.",
                _ => "Not available in this location."
            };
        }
        else
        {
            if (_latestPRI.Status == PriStatus.Abandoned)
            {
                _priDueTooltipText = "Pre-Release Inventory has been Abandoned.";
            }
            else if (_latestPRI.Status == PriStatus.Completed)
            {
                _priDueTooltipText = "Pre-Release Inventory has been Completed.";
            }
            else
            {
                if (_latestPRI.TTGDueDate.HasValue)
                {
                    _showTTGDue = true;
                    _priDueInfo = _latestPRI.TTGDueDate.Value.Humanize();
                    _priDueTooltipText = String.Format(ConstantString.PriTTGDueWarningToolTip, $"on {_latestPRI.TTGDueDate.Value}");

                    if (_latestPRI.IsFinalTTGWarningApplicable)
                    {
                        _priDueIcon = Icons.Material.Filled.Error;
                        _priDueIconColor = Color.Error;
                    }
                    else if (_latestPRI.IsFirstTTGWarningApplicable)
                    {
                        _priDueIcon = Icons.Material.Filled.Warning;
                        _priDueIconColor = Color.Warning;
                    }
                }
                else if (ParticipantSummaryDto.LocationType.IsCommunity)
                {
                    _priDueInfo = ConstantString.PriNoActualReleaseDateWarning;
                    _showTTGDue = true;
                }
            }
        }
    }

    void SetAssessmentDueWarning()
    {
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
}