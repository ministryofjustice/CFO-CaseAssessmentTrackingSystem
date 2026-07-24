using Cfo.Cats.Application.Features.Transfers.Commands;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.Features.Transfers.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class Transfers
{

    private bool _isLoadingIncomingTransfers = true;
    private bool _isLoadingOutgoingTransfers = true;

    private List<IncomingTransferDto> _incomingTransfers = [];
    private List<OutgoingTransferDto> _outgoingTransfers = [];

    private string _searchIncomingString = "";
    private string _searchOutgoingString = "";

    protected async override Task OnInitializedAsync()
    {
        _incomingTransfers = await GetIncomingTransfers();
        _outgoingTransfers = await GetOutgoingTransfers();

        await base.OnInitializedAsync();
    }

    private async Task<List<IncomingTransferDto>> GetIncomingTransfers()
    {
        _isLoadingIncomingTransfers = true;
        var query = await GetNewMediator().Send(new GetIncomingTransfers.Query());
        _isLoadingIncomingTransfers = false;
        return query.Data?.ToList() ?? [];
    }

    private async Task<List<OutgoingTransferDto>> GetOutgoingTransfers()
    {
        _isLoadingOutgoingTransfers = true;
        var query = await GetNewMediator().Send(new GetOutgoingTransfers.Query());
        _isLoadingOutgoingTransfers = false;
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
            _incomingTransfers.Remove(incomingTransfer);
        }
    }

    private async Task Dismiss(IncomingTransferDto incomingTransfer)
    {
        var parameters = new DialogParameters<DismissTransferDialog>
        {
            { x => x.Model, incomingTransfer }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var dialog = await DialogService.ShowAsync<DismissTransferDialog>("Dismiss Transfer", parameters, options);
        var result = await dialog.Result;

        if (result is not { Canceled: false })
        {
            return;
        }

        var command = new DismissIncomingTransfer.Command { IncomingTransfer = incomingTransfer };
        var dismissResult = await GetNewMediator().Send(command);

        if (dismissResult.Succeeded)
        {
            _incomingTransfers.Remove(incomingTransfer);
            Snackbar.Add("Transfer dismissed successfully.", Severity.Info);
        }
        else
        {
            Snackbar.Add(dismissResult.ErrorMessage, Severity.Error);
        }
    }

    private void View(string participantId) => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}?from=transfers");

    private async Task ViewOffenderManagerSummary(string participantId)
    {
        var parameters = new DialogParameters<OffenderManagerSummaryDialog> { { x => x.ParticipantId, participantId } };

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