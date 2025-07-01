using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries;

namespace Cfo.Cats.Server.UI.Pages.PRIs.Components;

public partial class ParticipantsToBeReleasedSoon
{
    [CascadingParameter]
    private UserProfile? UserProfile { get; set; } = default!;

    private GetSoonToBeReleasedParticipants.Query? SoonToBeReleasedParticipantsQuery { get; set; }

    private int _defaultPageSize = 15;
    private HashSet<SoonToBeReleasedPaginationDto> _selectedSoonToBeReleasedParticipants = new();
    private MudDataGrid<SoonToBeReleasedPaginationDto> _SoonToBeReleasedParticipantsTable = default!;
    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        SoonToBeReleasedParticipantsQuery = new GetSoonToBeReleasedParticipants.Query()
        {
            CurrentUser = UserProfile
        };


        await base.OnInitializedAsync();
    }

    private async Task<GridData<SoonToBeReleasedPaginationDto>> ServerReload(GridState<SoonToBeReleasedPaginationDto> state)
    {
        try
        {
            _loading = true;
            SoonToBeReleasedParticipantsQuery!.CurrentUser = UserProfile!;
            SoonToBeReleasedParticipantsQuery.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            SoonToBeReleasedParticipantsQuery.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            SoonToBeReleasedParticipantsQuery.PageNumber = state.Page + 1;
            SoonToBeReleasedParticipantsQuery.PageSize = state.PageSize;
            var result = await GetNewMediator().Send(SoonToBeReleasedParticipantsQuery);
            return new GridData<SoonToBeReleasedPaginationDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearchSoonToBeReleasedParticipants(string text)
    {
        if (_loading)
        {
            return;
        }

        await _SoonToBeReleasedParticipantsTable.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        _selectedSoonToBeReleasedParticipants = [];
        SoonToBeReleasedParticipantsQuery!.Keyword = string.Empty;
        await _SoonToBeReleasedParticipantsTable.ReloadServerData();
    }

    private void ViewParticipant(SoonToBeReleasedPaginationDto PRI)
    {
        Navigation.NavigateTo($"/pages/Participants/{PRI.ParticipantId}");
    }
}