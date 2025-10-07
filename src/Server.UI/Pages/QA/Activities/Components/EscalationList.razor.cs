using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities.Components;

public partial class EscalationList
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private bool _loading = false;
    private int _defaultPageSize = 30;
    private MudDataGrid<ActivityQueueEntryDto> _table = default!;

    private ActivityQaEscalationWithPaginiation.Query Query { get; set; } = new();
    private ActivityQueueEntryDto _currentDto = new();

    private void ViewActivity(ActivityQueueEntryDto dto)
    {
        Navigation.NavigateTo($"/pages/qa/activities/escalation/{dto.Id}");
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
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }
}