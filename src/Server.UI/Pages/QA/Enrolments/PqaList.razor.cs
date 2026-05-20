using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class PqaList
{
    [CascadingParameter] private UserProfile? UserProfile { get; set; }

    private bool _loading = false;
    private bool _downloading;
    private int _defaultPageSize = 30;
    private MudDataGrid<EnrolmentQueueEntryDto> _table = default!;

    private PqaQueueWithPagination.Query Query { get; set; } = new();
    private EnrolmentQueueEntryDto _currentDto = new();
    
    public string? SelectedTenantId { get; set; }
    public string? SelectedDisplayName { get; set; }
    public string? SelectedSupportWorkerId { get; set; }
    public string? SelectedSupportWorkerName { get; set; }
    
    private List<(string Id, string Name)> _availableSupportWorkers = new();

    protected override async Task OnInitializedAsync()
    {
        SelectedTenantId = UserProfile?.TenantId;
        SelectedDisplayName = UserProfile?.TenantName;
        SelectedSupportWorkerId = null;
        SelectedSupportWorkerName = "All Support Workers";
        
        await LoadAvailableSupportWorkers();
    }

    private async Task LoadAvailableSupportWorkers()
    {
        try
        {
            // Load all support workers for the selected tenant (without support worker filter)
            var effectiveUser = new UserProfile
            {
                UserId = UserProfile?.UserId ?? string.Empty,
                UserName = UserProfile?.UserName ?? string.Empty,
                Email = UserProfile?.Email ?? string.Empty,
                TenantId = SelectedTenantId,
                TenantName = SelectedDisplayName,
                AssignedRoles = UserProfile?.AssignedRoles ?? [],
                Contracts = UserProfile?.Contracts ?? []
            };
            
            var query = new PqaQueueWithPagination.Query
            {
                CurrentUser = effectiveUser,
                SupportWorkerId = null, // Don't filter by support worker
                PageNumber = 1,
                PageSize = 1000, // Get a large set to capture all support workers
                OrderBy = "Created",
                SortDirection = "Descending"
            };
            
            var result = await GetNewMediator().Send(query);
            
            _availableSupportWorkers = result.Items
                .Select(x => (x.SupportWorkerId, x.SupportWorker))
                .Distinct()
                .OrderBy(x => x.SupportWorker)
                .ToList();
        }
        catch
        {
            // If loading fails, just use empty list
            _availableSupportWorkers = new();
        }
    }

    private void RowClicked(DataGridRowClickEventArgs<EnrolmentQueueEntryDto> args) => Navigation.NavigateTo($"/pages/qa/enrolments/pqa/{args.Item.Id}");

    private async Task<GridData<EnrolmentQueueEntryDto>> ServerReload(GridState<EnrolmentQueueEntryDto> state, CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            
            // Create a user profile with the selected tenant for filtering
            var effectiveUser = new UserProfile
            {
                UserId = UserProfile?.UserId ?? string.Empty,
                UserName = UserProfile?.UserName ?? string.Empty,
                Email = UserProfile?.Email ?? string.Empty,
                TenantId = SelectedTenantId,
                TenantName = SelectedDisplayName,
                AssignedRoles = UserProfile?.AssignedRoles ?? [],
                Contracts = UserProfile?.Contracts ?? []
            };
            
            Query.CurrentUser = effectiveUser;
            Query.SupportWorkerId = SelectedSupportWorkerId;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Created";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true ? SortDirection.Descending.ToString() : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query);
            
            return new GridData<EnrolmentQueueEntryDto> { TotalItems = result.TotalItems, Items = result.Items };
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
        SelectedSupportWorkerId = null;
        SelectedSupportWorkerName = "All Support Workers";
        await _table.ReloadServerData();
    }

    private async Task OnSupportWorkerChanged(string? supportWorkerId)
    {
        if (_loading)
        {
            return;
        }
        
        SelectedSupportWorkerId = supportWorkerId;
        if (string.IsNullOrEmpty(supportWorkerId))
        {
            SelectedSupportWorkerName = "All Support Workers";
        }
        else
        {
            SelectedSupportWorkerName = _availableSupportWorkers
                .FirstOrDefault(x => x.Id == supportWorkerId).Name ?? "Unknown";
        }
        
        await _table.ReloadServerData();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportPqaEnrolments.Command()
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
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private async Task DisplayTenantSelectorDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", UserProfile! }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select Tenant", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            SelectedTenantId = tenant.TenantId;
            SelectedDisplayName = tenant.DisplayName;
            
            // Reload support workers list for the new tenant
            await LoadAvailableSupportWorkers();
            
            // Reset support worker filter when tenant changes
            SelectedSupportWorkerId = null;
            SelectedSupportWorkerName = "All Support Workers";
            
            await _table.ReloadServerData();
        }
    }
}