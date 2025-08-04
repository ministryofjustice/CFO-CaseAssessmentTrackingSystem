using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Risk;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class RiskSummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private string _riskIcon = String.Empty;

    protected override void OnInitialized()
    {
        if (ParticipantSummaryDto.RiskDue.HasValue)
        {
            int dueInDays = ParticipantSummaryDto.RiskDueInDays!.Value!;
            switch (dueInDays)
            {
                case var _ when dueInDays <= 0:
                    _riskIcon = Icons.Material.Filled.Error;
                    break;
                case >= 0 and <= 14:
                    _riskIcon = Icons.Material.Filled.Warning;
                    break;
            }
        }
    }

    private bool HasRiskInformation() => ParticipantSummaryDto.LatestRisk is not null;

    private bool CanAddRiskInformation()
    {
        return HasRiskInformation() is false
        && ParticipantSummaryDto.IsActive;
    }

    private bool CanReviewRiskInformation()
    {
        return HasRiskInformation()
        && ParticipantSummaryDto.IsActive;
    }

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
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
        else
        {
            if (command.ReviewReason.RequiresFurtherInformation)
            {
                Navigation.NavigateTo($"/pages/participants/{ParticipantSummaryDto.Id}/risk/{result.Data}");
            }
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

}
