using Cfo.Cats.Application.Features.Transfers.Commands;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.Features.Transfers.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class IncomingTransfers
{
    [Inject]
    public IParticipantDialogService ParticipantDialogService { get; set; } = null!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    private bool _isLoading = true;
    private List<IncomingTransferDto> _transfers = [];
    private string _searchString = "";
    private LocationDto? _locationFilter;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        var result = await GetNewMediator().Send(new GetIncomingTransfers.Query());
        _transfers = result.Data?.ToList() ?? [];
        _isLoading = false;

        await base.OnInitializedAsync();
    }

    private async Task Process(IncomingTransferDto incomingTransfer)
    {
        var command = new ProcessIncomingTransfer.Command { IncomingTransfer = incomingTransfer };
        var parameters = new DialogParameters<ProcessTransferDialog> { { x => x.Model, command } };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var dialog = await DialogService.ShowAsync<ProcessTransferDialog>("Process and Assign", parameters, options);
        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            var result = await GetNewMediator().Send(command);
            if (result.Succeeded)
            {
                dialog.Close();
            }

            _transfers.Remove(incomingTransfer);
        }
    }

    private async Task Dismiss(IncomingTransferDto incomingTransfer)
    {
        var parameters = new DialogParameters<DismissTransferDialog> { { x => x.Model, incomingTransfer } };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var dialog = await DialogService.ShowAsync<DismissTransferDialog>("Dismiss Transfer", parameters, options);
        var result = await dialog.Result;

        if (result is not { Canceled: false })
        {
            return;
        }

        var dismissResult = await GetNewMediator().Send(new DismissIncomingTransfer.Command { IncomingTransfer = incomingTransfer });
        if (dismissResult.Succeeded)
        {
            _transfers.Remove(incomingTransfer);
            Snackbar.Add("Transfer dismissed successfully.", Severity.Info);
        }
        else
        {
            Snackbar.Add(dismissResult.ErrorMessage, Severity.Error);
        }
    }

    private void View(string participantId) => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}?from=incoming-transfers");

    private async Task ViewOffenderManagerSummary(string participantId)
    {
        var parameters = new DialogParameters<OffenderManagerSummaryDialog> { { x => x.ParticipantId, participantId } };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        await DialogService.ShowAsync<OffenderManagerSummaryDialog>(ConstantString.OffenderManagerDeliusFeed, parameters, options);
    }

    private async Task ShowSelectLocationDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(UserProfile);

        if (location is not null)
        {
            _locationFilter = location;
        }
    }

    private void ClearSearch()
    {
        _searchString = string.Empty;
        _locationFilter = null;
    }

    private bool Filter(IncomingTransferDto transfer)
    {
        if (_locationFilter is not null
            && transfer.FromLocation.Id != _locationFilter.Id
            && transfer.ToLocation.Id != _locationFilter.Id)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        return transfer.ParticipantId.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.ParticipantFullName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || LocationValues(transfer).Any(value => value?.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) == true);
    }

    private static string?[] LocationValues(IncomingTransferDto transfer) =>
    [
        transfer.FromLocation.Name,
        transfer.FromLocation.ParentLocationName,
        transfer.FromLocation.ContractName,
        transfer.ToLocation.Name,
        transfer.ToLocation.ParentLocationName,
        transfer.ToLocation.ContractName
    ];
}