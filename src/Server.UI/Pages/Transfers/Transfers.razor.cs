using Cfo.Cats.Application.Features.Transfers.Commands;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.Features.Transfers.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;
using Cfo.Cats.Server.UI.Pages.Transfers.Components;

namespace Cfo.Cats.Server.UI.Pages.Transfers;

public partial class Transfers
{

    private bool isLoadingIncomingTransfers = true;
    private bool isLoadingOutgoingTransfers = true;

    private List<IncomingTransferDto> incomingTransfers = [];
    private List<OutgoingTransferDto> outgoingTransfers = [];

    private string _searchIncomingString = "";
    private string _searchOutgoingString = "";

    protected async override Task OnInitializedAsync()
    {
        incomingTransfers = await GetIncomingTransfers();
        outgoingTransfers = await GetOutgoingTransfers();

        await base.OnInitializedAsync();
    }

    private async Task<List<IncomingTransferDto>> GetIncomingTransfers()
    {
        isLoadingIncomingTransfers = true;
        var query = await GetNewMediator().Send(new GetIncomingTransfers.Query());
        isLoadingIncomingTransfers = false;
        return query.Data?.ToList() ?? [];
    }

    private async Task<List<OutgoingTransferDto>> GetOutgoingTransfers()
    {
        isLoadingOutgoingTransfers = true;
        var query = await GetNewMediator().Send(new GetOutgoingTransfers.Query());
        isLoadingOutgoingTransfers = false;
        return query.Data?.ToList() ?? [];
    }

    private async Task Process(IncomingTransferDto incomingTransfer)
    {
        var command = new ProcessIncomingTransfer.Command()
        {
            IncomingTransfer = incomingTransfer
        };

        var parameters = new DialogParameters<ProcessTransferDialog>()
        {
            { x => x.Model, command }
        };

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

            // Remove dto from UI
            incomingTransfers.Remove(incomingTransfer);
        }
    }

    private async Task Dismiss(IncomingTransferDto incomingTransfer)
    {
        var parameters = new DialogParameters<ConfirmationDialog>
        {
            { x => x.ContentText, $"Are you sure you want to dismiss the incoming transfer for {incomingTransfer.ParticipantFullName}? This cannot be undone." }
        };

        var dialog = await DialogService.ShowAsync<ConfirmationDialog>("Dismiss Transfer", parameters);
        var result = await dialog.Result;

        if (result is not { Canceled: false })
        {
            return;
        }

        var command = new DismissIncomingTransfer.Command { IncomingTransfer = incomingTransfer };
        var dismissResult = await GetNewMediator().Send(command);

        if (dismissResult.Succeeded)
        {
            incomingTransfers.Remove(incomingTransfer);
            Snackbar.Add("Transfer dismissed successfully.", Severity.Info);
        }
        else
        {
            Snackbar.Add(dismissResult.ErrorMessage, Severity.Error);
        }
    }

    private void View(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");

    private async Task ViewOffenderManagerSummary(string participantId)
    {
        var parameters = new DialogParameters<OffenderManagerSummaryDialog>();
        parameters.Add(x => x.ParticipantId, participantId);

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };

        await DialogService.ShowAsync<OffenderManagerSummaryDialog>(ConstantString.OffenderManagerDeliusFeed, parameters, options);

    }

    private bool FilterIncoming1(IncomingTransferDto arg)
        => FilterIncoming(arg, _searchIncomingString);

    private bool FilterIncoming(IncomingTransferDto arg, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return true;
        }

        if (arg.ParticipantId.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (arg.ParticipantFullName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (arg.FromLocation.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (arg.ToLocation.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }
        return false;
    }

    private bool FilterOutgoing1(OutgoingTransferDto arg)
        => FilterOutgoing(arg, _searchOutgoingString);

    private bool FilterOutgoing(OutgoingTransferDto arg, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return true;
        }

        if (arg.ParticipantId.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (arg.ParticipantFullName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (arg.FromLocation.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (arg.ToLocation.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }
        return false;
    }

}