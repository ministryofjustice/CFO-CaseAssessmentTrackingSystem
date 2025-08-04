using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Humanizer;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class PRISummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private PriSummaryDto? _latestPRI = null;

    private bool _showTTGDue;
    private string _noPriInfo = String.Empty;
    private string _priDueInfo = String.Empty;
    private string _priDueTooltipText = String.Empty;
    private string _priDueIcon = String.Empty;
    private Color _priDueIconColor = Color.Transparent;

    protected override void OnParametersSet()
    {
        _latestPRI = ParticipantSummaryDto.LatestPri;
    }

    protected override void OnInitialized()
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

    private bool CanAddPRI()
    {
        return _latestPRI == null
            && ParticipantSummaryDto.LocationType.IsCustody
            && ParticipantSummaryDto.LocationType.IsMapped
            && ParticipantSummaryDto.IsActive;
    }

    public void BeginPRI()
    {
        Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/PRI");
    }

}
