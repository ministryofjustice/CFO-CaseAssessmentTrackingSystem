using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.AuditTrails.Caching;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;
using Cfo.Cats.Application.Features.AuditTrails.Queries.GetSystemAuditTrailsWithPagination;
using Cfo.Cats.Application.Features.AuditTrails.Specifications.SystemAuditTrail;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components;

public partial class SystemAuditTrails
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private AuditTrailsWithPaginationQuery Query { get; } = new();
    private MudDataGrid<AuditTrailDto> _table = null!;
    private bool _loading;
    private int _defaultPageSize = 15;
    private readonly AuditTrailDto _currentDto = new();

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
    }

    private async Task<GridData<AuditTrailDto>> ServerReload(GridState<AuditTrailDto> state,
        CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                ? nameof(SortDirection.Descending)
                : nameof(SortDirection.Ascending);
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await Mediator.Send(Query, cancellationToken: cancellationToken);
            return new GridData<AuditTrailDto> { TotalItems = result.TotalItems, Items = result.Items };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnChangedListView(SystemAuditTrailListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(AuditType? val)
    {
        Query.AuditType = val;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        SystemAuditTrailsCacheKey.Refresh();
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private Task OnShowDetail(AuditTrailDto dto)
    {
        dto.ShowDetails = !dto.ShowDetails;
        return Task.CompletedTask;
    }
}