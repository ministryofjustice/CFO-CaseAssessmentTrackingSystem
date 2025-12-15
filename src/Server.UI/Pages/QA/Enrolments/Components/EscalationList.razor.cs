using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components;

public partial class EscalationList
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private bool _loading;
    private int _defaultPageSize = 30;
    private QaEscalationWithPaginiation.Query Query { get; } = new();
    private GridData<EnrolmentQueueEntryDto>? _pagedData;
    private int _currentPage;
    
    private HashSet<Guid> ExpandedRows { get; } = [];
    
    private int TotalPages =>
        (_pagedData!.TotalItems + _defaultPageSize - 1) / _defaultPageSize;
   
    private async Task OnPaginationChanged(int page)
    {
        _currentPage = page - 1;
        await LoadPage();
    }
    
    private async Task LoadPage()
    {
        var state = new GridState<EnrolmentQueueEntryDto>
        {
            Page = _currentPage,
            PageSize = _defaultPageSize
        };

        _pagedData = await ServerReload(state);
        StateHasChanged();
    }

    private async Task<GridData<EnrolmentQueueEntryDto>> ServerReload(GridState<EnrolmentQueueEntryDto> state)
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
            return new GridData<EnrolmentQueueEntryDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }
    
    private void ViewParticipant(EnrolmentQueueEntryDto dto) => Navigation.NavigateTo($"/pages/participants/{dto.ParticipantId}");
    
    private void ViewEnrolment(EnrolmentQueueEntryDto dto) => Navigation.NavigateTo($"/pages/qa/enrolments/escalation/{dto.Id}");

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
    
    private void ToggleRow(Guid activityId)
    {
        if (!ExpandedRows.Remove(activityId))
        {
            ExpandedRows.Add(activityId);
        }

        StateHasChanged();
    }
}