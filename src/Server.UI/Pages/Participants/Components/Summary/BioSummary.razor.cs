using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Bios.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Humanizer;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class BioSummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private string _bioInfo = String.Empty;
    private string _bioTooltipText = String.Empty;
    private string _bioIcon = String.Empty;
    private Color _bioIconColor = Color.Transparent;

    private BioSummaryDto? _bio;

    protected override void OnParametersSet()
    {
        _bio = ParticipantSummaryDto.BioSummary;

    }

    protected override void OnInitialized()
    {
        if (ParticipantSummaryDto.BioDue.HasValue)
        {
            var datePart = ParticipantSummaryDto.BioDue.Value.Date;

            _bioInfo = datePart.Humanize();
            _bioTooltipText = String.Format("Due {0}", DateOnly.FromDateTime(datePart));

            int _dueInDays = ParticipantSummaryDto.BioDueInDays!.Value!;
            switch (_dueInDays)
            {
                case var _ when _dueInDays <= 0:
                    _bioIcon = Icons.Material.Filled.Error;
                    _bioIconColor = Color.Error;
                    _bioTooltipText = String.Format("Overdue {0}", DateOnly.FromDateTime(datePart));
                    break;
                case var _ when _dueInDays >= 0 && _dueInDays <= 14:
                    _bioIcon = Icons.Material.Filled.Warning;
                    _bioIconColor = Color.Warning;
                    _bioTooltipText = String.Format("Due Soon {0}", DateOnly.FromDateTime(datePart));
                    break;
            }
        }
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

    public void ContinueBio()
    {
        Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/bio/{_bio!.BioId}");
    }

    /// <summary>
    /// If true, indicates we are creating Bio. 
    /// </summary>
    private bool CanBeginBio()
    {
        return _bio == null
                && ParticipantSummaryDto.IsActive;
    }

    private bool CanRestartBio()
    {
        return _bio?.BioStatus == BioStatus.Complete
               && ParticipantSummaryDto.IsActive;
    }

    /// <summary>
    /// If true indicates we have a Bio that is continuable
    /// (i.e. Id is not null or do we need a status (Complete or Incomplete etc.))
    /// </summary>
    /// <returns></returns>
    private bool CanContinueBio()
    {
        return _bio is not null
            && (_bio.BioStatus == BioStatus.InProgress || _bio.BioStatus == BioStatus.SkippedForNow)
            && ParticipantSummaryDto.IsActive;
    }

}
