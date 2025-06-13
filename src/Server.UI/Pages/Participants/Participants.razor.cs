using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Export;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Participants;

public partial class Participants
{
    [Inject] public ILocationService LocationService { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "ListView")]
    public string? ListView { get; set; }

    public string? Title { get; private set; }
    private int _defaultPageSize = 15;
    public LocationDto[] Locations { get; set; } = [];
    private HashSet<ParticipantPaginationDto> _selectedItems = new();
    private MudDataGrid<ParticipantPaginationDto> _table = default!;
    private bool _loading;
    private bool _downloading;
    private bool _canSearch;
    private bool _canEnrol;
    private ParticipantPaginationDto _currentDto = new() { Id = "" };
    private ParticipantsWithPagination.Query? Query { get; set; }

    private void OnResumeEnrolment(ParticipantPaginationDto participant)
    {
        Navigation.NavigateTo($"/pages/enrolments/{participant.Id}");
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        return $"{selectedValues.Count} location{(selectedValues.Count == 1 ? " has" : "s have")} been selected";
    }

    private async Task LocationValuesChanged(IEnumerable<LocationDto>? selectedValues)
    {
        Query!.Locations = selectedValues!.Select(l => l.Id).ToArray();
        await _table.ReloadServerData();
    }

    private void Edit(ParticipantPaginationDto participant)
    {
        Navigation.NavigateTo($"/pages/participants/{participant.Id}");
    }

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private async Task OnShowEveryoneChanged(bool value)
    {
        Query!.JustMyCases = value;
        await _table.ReloadServerData();
    }

    protected override async Task OnInitializedAsync()
    {
        Title = L["Participants"];
        var state = await AuthState;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.CandidateSearch)).Succeeded;
        _canEnrol = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Enrol)).Succeeded;

        Enum.TryParse<ParticipantListView>(ListView, true, out var selectedListView);

        Query = new ParticipantsWithPagination.Query()
        {
            JustMyCases = UserProfile!.AssignedRoles.Length == 0,
            ListView = selectedListView,
        };

        Locations = LocationService.GetVisibleLocations(UserProfile!.TenantId!)
            .OrderByDescending(l => l.LocationType.IsCustody)
            .ThenBy(l => l.Name).ToArray();
    }

    private async Task<GridData<ParticipantPaginationDto>> ServerReload(GridState<ParticipantPaginationDto> state)
    {
        try
        {
            _loading = true;
            Query!.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? SortDirection.Descending.ToString()
                : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            var result = await GetNewMediator().Send(Query).ConfigureAwait(false);

            if (result is {Succeeded: true, Data: not null})
            {
                return new GridData<ParticipantPaginationDto> { TotalItems = result.Data.TotalItems, Items = result.Data.Items };
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Warning);
                return new GridData<ParticipantPaginationDto> { TotalItems = 0, Items = [] };
            }
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearch(string text)
    {
        if (_loading)
        {
            return;
        }

        _selectedItems = new();
        Query!.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task OnChangedListView(ParticipantListView listview)
    {
        Query!.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        ParticipantCacheKey.Refresh();
        _selectedItems = [];
        Query!.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private void OnEnrol()
    {
        Navigation.NavigateTo("/pages/candidates/search");
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportParticipants.Command()
            {
                Query = Query!
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);

        }
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

}