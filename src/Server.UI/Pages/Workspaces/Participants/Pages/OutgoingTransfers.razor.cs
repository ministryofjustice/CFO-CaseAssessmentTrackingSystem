using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.Features.Transfers.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class OutgoingTransfers
{
    [Inject]
    public IParticipantDialogService ParticipantDialogService { get; set; } = null!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    private bool _isLoading = true;
    private List<OutgoingTransferDto> _transfers = [];
    private string _searchString = "";
    private LocationDto? _locationFilter;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        var result = await GetNewMediator().Send(new GetOutgoingTransfers.Query());
        _transfers = result.Data?.ToList() ?? [];
        _isLoading = false;

        await base.OnInitializedAsync();
    }

    private void View(string participantId) => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}?from=outgoing-transfers");

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

    private bool Filter(OutgoingTransferDto transfer)
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
            || LocationValues(transfer).Any(value => value?.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) == true)
            || (transfer.PreviousSupportWorkerName?.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) ?? false);
    }

    private static string?[] LocationValues(OutgoingTransferDto transfer) =>
    [
        transfer.FromLocation.Name,
        transfer.FromLocation.ParentLocationName,
        transfer.FromLocation.ContractName,
        transfer.ToLocation.Name,
        transfer.ToLocation.ParentLocationName,
        transfer.ToLocation.ContractName
    ];
}