using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Identity.Queries.PaginationQuery;
using Cfo.Cats.Application.Features.Identity.Specifications;


namespace Cfo.Cats.Server.UI.Pages.Identity.Users;

public partial class LoginAudit : CatsComponentBase
{
    private MudDataGrid<IdentityAuditTrailDto> _table = null!;
    private bool _loading;
    private readonly IdentityAuditTrailDto _currentDto = new();
    private int _defaultPageSize = 15;

    private string Title { get; set; } = "User Audit";
    private IdentityAuditTrailsWithPagination.Query Query { get; } = new();

    private async Task OnChangedListView(IdentityAuditTrailListView listview)
    {
        Query!.ListView = listview;
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
    
    private async Task<GridData<IdentityAuditTrailDto>> ServerReload(GridState<IdentityAuditTrailDto> state)
    {
        try
        {
            _loading = true;
            Query.UserName = null;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? SortDirection.Descending.ToString()
                : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query);
            if (result is { Succeeded: true, Data: not null })
            {
                return new GridData<IdentityAuditTrailDto> { TotalItems = result.Data.TotalItems, Items = result.Data.Items };    
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return new GridData<IdentityAuditTrailDto>() { TotalItems = 0, Items = [] };
        }
        finally
        {
            _loading = false;
        }
    }
    
}

