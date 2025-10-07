using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities;

public partial class PqaList
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private bool _loading = false;
    private bool _downloading;
    private int _defaultPageSize = 30;
    private MudDataGrid<ActivityQueueEntryDto> _table = default!;

    private ActivityPqaQueueWithPagination.Query Query { get; set; } = new();
    private ActivityQueueEntryDto _currentDto = new();
    
    private void RowClicked(DataGridRowClickEventArgs<ActivityQueueEntryDto> args)
    {
        Navigation.NavigateTo($"/pages/qa/activities/pqa/{args.Item.Id}");
    }

    private async Task<GridData<ActivityQueueEntryDto>> ServerReload(GridState<ActivityQueueEntryDto> state)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "CommencedOn";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? false ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
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

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportPqaActivities.Command()
            {
                Query = Query
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error has occurred while generating the PQA Export");
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}