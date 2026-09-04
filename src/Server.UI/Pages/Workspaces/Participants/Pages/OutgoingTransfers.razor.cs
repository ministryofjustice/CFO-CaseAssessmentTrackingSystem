using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.Features.Transfers.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class OutgoingTransfers
{
    private bool _isLoading = true;
    private List<OutgoingTransferDto> _transfers = [];
    private string _searchString = "";

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        var result = await GetNewMediator().Send(new GetOutgoingTransfers.Query());
        _transfers = result.Data?.ToList() ?? [];
        _isLoading = false;

        await base.OnInitializedAsync();
    }

    private void View(string participantId) => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}?from=outgoing-transfers");

    private bool Filter(OutgoingTransferDto transfer)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        return transfer.ParticipantId.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.ParticipantFullName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.FromLocation.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || transfer.ToLocation.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase)
            || (transfer.PreviousSupportWorkerName?.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) ?? false);
    }
}