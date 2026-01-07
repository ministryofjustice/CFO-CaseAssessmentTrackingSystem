using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components;

public partial class QA2List
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private bool _loading;
    private int _defaultPageSize = 30;
    private MudDataGrid<EnrolmentQueueEntryDto> _table = null!;

    private Qa2WithPagination.Query Query { get; } = new();
    
    private void OnEdit(EnrolmentQueueEntryDto dto) => Navigation.NavigateTo($"/pages/participants/{dto.ParticipantId}");

    private async Task<GridData<EnrolmentQueueEntryDto>> ServerReload(GridState<EnrolmentQueueEntryDto> state)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Created";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? nameof(SortDirection.Descending) : nameof(SortDirection.Ascending);
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

    private async Task OnSearch(string text)
    {
        if (_loading)
        {
            return;
        }
 
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }
}