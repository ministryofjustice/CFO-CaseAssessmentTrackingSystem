using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Identity.Commands;
using Cfo.Cats.Application.Features.Identity.Queries.PaginationQuery;
using Cfo.Cats.Application.Features.Identity.Specifications;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages.Users;

public partial class UserAudit : CatsComponentBase
{
    private MudTable<IdentityAuditTrailDto> _table = null!;
    private bool _loading;
    private bool _downloading;
    private readonly IdentityAuditTrailDto _currentDto = new();
    private int _defaultPageSize = 15;

    private string Title { get; set; } = "User Audit";
    private IdentityAuditTrailsWithPagination.Query Query { get; } = new();

    private async Task OnChangedListView(IdentityAuditTrailListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(IdentityActionType? identityActionType)
    {
        Query.IdentityActionType = identityActionType;
        await _table.ReloadServerData();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportIdentityAuditTrails.Command
            {
                Request = new ExportIdentityAuditTrails.IdentityAuditTrailsExportRequest
                {
                    IdentityActionType = Query.IdentityActionType,
                    ListView = Query.ListView,
                    UserName = Query.UserName,
                    OrderBy = Query.OrderBy,
                    SortDirection = Query.SortDirection
                }
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add("An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
    
    private async Task<TableData<IdentityAuditTrailDto>> ServerReload(TableState state, CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.UserName = null;
            Query.OrderBy = string.IsNullOrWhiteSpace(state.SortLabel) ? "Id" : state.SortLabel;
            Query.SortDirection = state.SortDirection == SortDirection.Descending
                ? nameof(SortDirection.Descending)
                : nameof(SortDirection.Ascending);
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query, cancellationToken: cancellationToken);
            if (result is { Succeeded: true, Data: not null })
            {
                return new TableData<IdentityAuditTrailDto> { TotalItems = result.Data.TotalItems, Items = result.Data.Items };
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return new TableData<IdentityAuditTrailDto> { TotalItems = 0, Items = [] };
        }
        finally
        {
            _loading = false;
        }
    }
}
