using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;

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
    
    public string? SelectedTenantId { get; set; }
    public string? SelectedDisplayName { get; set; }
    public string? SelectedSupportWorkerId { get; set; }
    public string? SelectedSupportWorkerName { get; set; }
    public int? SelectedActivityTypeId { get; set; }
    public string? SelectedActivityTypeName { get; set; }
    
    private List<(string Id, string Name)> _availableSupportWorkers = new();
    private readonly List<ActivityType> _availableActivityTypes = ActivityType.List.ToList();

    protected override async Task OnInitializedAsync()
    {
        SelectedTenantId = UserProfile?.TenantId;
        SelectedDisplayName = UserProfile?.TenantName;
        SelectedSupportWorkerId = null;
        SelectedSupportWorkerName = "All Support Workers";
        SelectedActivityTypeId = null;
        SelectedActivityTypeName = "All Activity Types";
        
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
            
            var query = new ActivityPqaQueueWithPagination.Query
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
                .Where(x => !string.IsNullOrEmpty(x.SupportWorkerId))
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

    private void RowClicked(DataGridRowClickEventArgs<ActivityQueueEntryDto> args) => Navigation.NavigateTo($"/pages/qa/activities/pqa/{args.Item.Id}");

    private async Task<GridData<ActivityQueueEntryDto>> ServerReload(GridState<ActivityQueueEntryDto> state, CancellationToken cancellationToken)
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
            Query.ActivityTypeId = SelectedActivityTypeId;
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
        SelectedTenantId = UserProfile?.TenantId;
        SelectedDisplayName = UserProfile?.TenantName;
        SelectedSupportWorkerId = null;
        SelectedSupportWorkerName = "All Support Workers";
        SelectedActivityTypeId = null;
        SelectedActivityTypeName = "All Activity Types";
        await LoadAvailableSupportWorkers();
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

    private async Task OnActivityTypeChanged(int? activityTypeId)
    {
        if (_loading)
        {
            return;
        }
        
        SelectedActivityTypeId = activityTypeId;
        if (activityTypeId == null)
        {
            SelectedActivityTypeName = "All Activity Types";
        }
        else
        {
            SelectedActivityTypeName = ActivityType.FromValue(activityTypeId.Value).Name;
        }
        
        await _table.ReloadServerData();
    }
    
    private async Task ActivityTypeChanged(int? activityTypeId) => await OnActivityTypeChanged(activityTypeId);

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

    private async Task ShowTenantDialog()
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
    
    private async Task ShowSupportWorkerDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", UserProfile! }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select Support Worker", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedUser user })
        {
            SelectedSupportWorkerId = user.UserId;
            SelectedSupportWorkerName = user.DisplayName;
            await _table.ReloadServerData();
        }
    }
}