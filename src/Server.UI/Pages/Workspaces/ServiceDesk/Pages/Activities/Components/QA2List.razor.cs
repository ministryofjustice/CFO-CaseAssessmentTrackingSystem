using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Application.Features.Activities.Commands;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Activities.Components;

public partial class QA2List
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    [Inject]
    public IParticipantDialogService ParticipantDialogService { get; set; } = null!;

    private bool _loading;  
    private int _defaultPageSize = 30;
    private MudDataGrid<ActivityQueueEntryDto> _table = null!;

    private ActivityQa2WithPagination.Query Query { get; } = new();
    
    private void ViewParticipant(ActivityQueueEntryDto dto) => Navigation.NavigateTo($"/pages/workspace/participants/{dto.ParticipantId}?from=activities-queue&tab=second-pass");

    public async Task Reassign(ActivityQueueEntryDto dto)
    {
        string[] tenants = ["1.", "1.1."];

        var newAssingee = await ParticipantDialogService.PromptForAssigneeAsync(UserProfile!, filter: x => tenants.Contains(x.TenantId!));

        if(newAssingee is not null)
        {
            var command = new ReassignQaEntry.Command()
            {
                NewUserId = newAssingee.UserId,
                QueueEntryId = dto.Id  
            };

            var result = await GetNewMediator().Send(command);

            if(result.Succeeded)
            {
                Snackbar.Add($"Reassigned to {newAssingee.DisplayName}");
                await OnRefresh();   
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, severity: MudBlazor.Severity.Error);
            }
        }
    }

    private async Task<GridData<ActivityQueueEntryDto>> ServerReload(GridState<ActivityQueueEntryDto> state, CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Created";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? nameof(SortDirection.Descending) : nameof(SortDirection.Ascending);
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
