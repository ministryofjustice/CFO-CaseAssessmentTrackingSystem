using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Identity.Queries.PaginationQuery;
using Cfo.Cats.Application.Features.Identity.Specifications;

namespace Cfo.Cats.Server.UI.Pages.Identity.Users.Components;

public partial class UserAuditDialog
{
    [Parameter, EditorRequired] public string UserName { get; set; } = default!;

    private IdentityAuditTrailsWithPagination.Query Query { get; } = new();
    private MudDataGrid<IdentityAuditTrailDto> _table = null!;
    private readonly IdentityAuditTrailDto _currentDto = new();
    private bool _loading;
    private int _defaultPageSize = 15;

    private async Task<GridData<IdentityAuditTrailDto>> ServerReload(GridState<IdentityAuditTrailDto> state)
    {
        try
        {
            _loading = true;
            Query.UserName = UserName;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? SortDirection.Descending.ToString()
                : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query).ConfigureAwait(false);
            return new GridData<IdentityAuditTrailDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnChangedListView(IdentityAuditTrailListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        await _table.ReloadServerData();
    }

    private async Task OnSearch(IdentityActionType? identityActionType)
    {
        Query.IdentityActionType = identityActionType;
        await _table.ReloadServerData();
    }
}