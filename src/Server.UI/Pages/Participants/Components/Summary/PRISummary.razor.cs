using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Humanizer;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class PRISummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = null!;

    [Inject]
    private IUserService UserService { get; set; } = null!;

    private PriSummaryDto? _latestPri;

    private bool _showTtgDue;
    private string _noPriInfo = String.Empty;
    private string _priDueInfo = String.Empty;
    private string _priDueTooltipText = String.Empty;
    private string _priDueIcon = String.Empty;
    private Color _priDueIconColor = Color.Transparent;

    protected override void OnParametersSet() => _latestPri = ParticipantSummaryDto.LatestPri;

    protected override void OnInitialized()
    {
        _priDueIcon = String.Empty;
        _priDueIconColor = Color.Transparent;

        if (_latestPri is null)
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
            if (_latestPri.Status == PriStatus.Abandoned)
            {
                _priDueTooltipText = "Pre-Release Inventory has been Abandoned.";
            }
            else if (_latestPri.Status == PriStatus.Completed)
            {
                _priDueTooltipText = "Pre-Release Inventory has been Completed.";
            }
            else
            {
                if (_latestPri.TTGDueDate.HasValue)
                {
                    _showTtgDue = true;
                    _priDueInfo = _latestPri.TTGDueDate.Value.Humanize();
                    _priDueTooltipText = String.Format(ConstantString.PriTTGDueWarningToolTip, $"on {_latestPri.TTGDueDate.Value}");

                    if (_latestPri.IsFinalTTGWarningApplicable)
                    {
                        _priDueIcon = Icons.Material.Filled.Error;
                        _priDueIconColor = Color.Error;
                    }
                    else if (_latestPri.IsFirstTTGWarningApplicable)
                    {
                        _priDueIcon = Icons.Material.Filled.Warning;
                        _priDueIconColor = Color.Warning;
                    }
                }
                else if (ParticipantSummaryDto.LocationType.IsCommunity)
                {
                    _priDueInfo = ConstantString.PriNoActualReleaseDateWarning;
                    _showTtgDue = true;
                }
            }
        }
    }

    private bool CanAddPri() =>
        _latestPri == null
        && ParticipantSummaryDto.LocationType.IsCustody
        && ParticipantSummaryDto.LocationType.IsMapped
        && ParticipantSummaryDto.IsActive;

    public void BeginPri() => Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/PRI");
}