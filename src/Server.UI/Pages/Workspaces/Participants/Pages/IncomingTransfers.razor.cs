using Cfo.Cats.Application.Features.Transfers.Commands;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.Features.Transfers.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class IncomingTransfers
{
    private bool _isLoading = true;
    private List<IncomingTransferDto> _transfers = [];
    private string _searchString = "";

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

    private bool Filter(IncomingTransferDto transfer)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        return transfer.ParticipantId.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.ParticipantFullName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.FromLocation.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.ToLocation.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase);
    }
}