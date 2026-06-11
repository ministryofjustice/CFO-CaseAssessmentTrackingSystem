using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.AuditTrails.DTOs;
using Cfo.Cats.Application.Features.AuditTrails.Queries;
using Cfo.Cats.Application.Features.AuditTrails.Specifications.DocumentAuditTrail;
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Server.UI.Pages.Analytics.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.SystemManagement.Components;

public partial class DocumentAuditTrails(IMediator mediator)
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter]
    private UserProfile? UserProfile { get; set; }

    private GetDocumentAuditTrailsWithPagination.Query Query { get; } = new();

    private MudDataGrid<DocumentAuditTrailDto> _table = null!;
    private bool _loading;
    private int _defaultPageSize = 15;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
    }

    private async Task<GridData<DocumentAuditTrailDto>> ServerReload(GridState<DocumentAuditTrailDto> state, CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await mediator.Send(Query, cancellationToken);

            if (result.Succeeded)
            {
                return new GridData<DocumentAuditTrailDto>
                    { TotalItems = result.Data!.TotalItems, Items = result.Data.Items };
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return new GridData<DocumentAuditTrailDto> { TotalItems = 0, Items = [] };

        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnChangedListView(DocumentAuditTrailListView listview)
    {
        Query.ListView = listview;
        await _table.ReloadServerData();
    }
    
    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(DocumentAuditTrailRequestType? val)
    {
        Query.RequestType = val;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        await _table.ReloadServerData();
    }

    private async Task Download(DocumentAuditTrailDto document)
    {
        var parameters = new DialogParameters<OnExportConfirmationDialog>()
        {
            { x => x.DocumentId, document.DocumentId }
        };

        var dialog = await DialogService.ShowAsync<OnExportConfirmationDialog>(document.DocumentTitle, parameters);

        await dialog.Result;
    }
}
