using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class DipSample
{
    private bool _loading;
    private MudDataGrid<ParticipantPaginationDto> _table = new();

    [Parameter]
    public required Guid SampleId { get; set; }

    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    ParticipantsWithPagination.Query Query { get; set; } = new();

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
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
            var result = await GetNewMediator().Send(Query);

            if (result is { Succeeded: true, Data: not null })
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

        Query!.Keyword = text;
        await _table.ReloadServerData();
    }
}
