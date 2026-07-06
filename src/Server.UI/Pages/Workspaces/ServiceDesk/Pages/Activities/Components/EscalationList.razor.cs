using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Activities.Components;

public partial class EscalationList
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private bool _loading;
    private int _defaultPageSize = 30;
    private MudDataGrid<ActivityQueueEntryDto> _table = null!;

    private ActivityQaEscalationWithPagination.Query Query { get; } = new();
  
    private void ViewActivity(ActivityQueueEntryDto dto) => Navigation.NavigateTo($"/pages/workspace/servicedesk/activities/escalation/{dto.Id}?from=activities-queue");

    private void ViewParticipant(ActivityQueueEntryDto dto) => Navigation.NavigateTo($"/pages/workspace/participants/{dto.ParticipantId}?from=activities-queue");

    private async Task<GridData<ActivityQueueEntryDto>> ServerReload(GridState<ActivityQueueEntryDto> state, CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Created";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query, cancellationToken);

            if (result.Succeeded)
            {
                return new GridData<ActivityQueueEntryDto>
                    { TotalItems = result.Data!.TotalItems, Items = result.Data.Items };
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return new GridData<ActivityQueueEntryDto> { TotalItems = 0, Items = [] };
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
