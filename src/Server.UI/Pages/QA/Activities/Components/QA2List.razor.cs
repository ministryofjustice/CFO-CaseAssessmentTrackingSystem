using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities.Components;

public partial class QA2List
{
    [CascadingParameter] public UserProfile? UserProfile { get; set; }

    private bool _loading;
    private int _defaultPageSize = 30;
    
    private ActivityQa2WithPagination.Query Query { get; } = new();
    private GridData<ActivityQueueEntryDto>? _pagedData;
    private int _currentPage;

    private int TotalPages =>
        (_pagedData!.TotalItems + _defaultPageSize - 1) / _defaultPageSize;
    
    private async Task OnPaginationChanged(int page)
    {
        _currentPage = page - 1;
        await LoadPage();
    }
    
    private async Task LoadPage()
    {
        var state = new GridState<ActivityQueueEntryDto>
        {
            Page = _currentPage,
            PageSize = _defaultPageSize
        };

        _pagedData = await ServerReload(state);
        StateHasChanged();
    }
    
    private void ViewParticipant(ActivityQueueEntryDto dto)
    {
        Navigation.NavigateTo($"/pages/participants/{dto.ParticipantId}");
    }
    
    private async Task<GridData<ActivityQueueEntryDto>> ServerReload(GridState<ActivityQueueEntryDto> state)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Created";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query);
            return new GridData<ActivityQueueEntryDto> { TotalItems = result.TotalItems, Items = result.Items };
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
        
        Query.Keyword = text;
        _currentPage = 0;

        await LoadPage();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        _currentPage = 0;

        await LoadPage();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadPage();
        }
    }
    
    // track expanded rows by ActivityId
    private HashSet<Guid> ExpandedRows { get; } = new();

    private void ToggleRow(Guid activityId)
    {
        if (ExpandedRows.Contains(activityId))
        {
            ExpandedRows.Remove(activityId);
        }
        else
        {
            ExpandedRows.Add(activityId);
        }

        StateHasChanged();
    }
}