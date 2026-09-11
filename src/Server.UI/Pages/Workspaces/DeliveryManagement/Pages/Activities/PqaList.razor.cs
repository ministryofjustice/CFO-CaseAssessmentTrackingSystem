using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages.Activities;

public partial class PqaList
{
    [CascadingParameter] private UserProfile UserProfile { get; set; } = null!;

    [Inject]
    public ITenantService TenantService { get; set; } = null!;

    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    private IDictionary<string, string> _users = new Dictionary<string, string>();
    private IDictionary<string, string> _tenants = null!;
    private string? _usersFilterSignature;

    private int _totalPages;
    private int _totalItems;

    private bool _loading;
    private bool _downloading;

    private ActivityQueueEntryDto[] _data = [];

    private ActivityPqaQueueWithPagination.Query Query { get; set; } = new();

    private readonly List<ActivityType> _availableActivityTypes = ActivityDefinition.List
        .Where(x => x.RequiresQa)
        .GroupBy(x => x.Type)
        .Select(x => x.Key)
        .ToList();

    private string SelectedActivityTypeName
        => Query.ActivityTypeId is null
            ? "All Activity Types"
            : _availableActivityTypes.FirstOrDefault(a => a.Value == Query.ActivityTypeId.Value)?.Name ?? "Unknown Activity Type";

    protected override async Task OnInitializedAsync()
    {
        _tenants = TenantService.GetVisibleTenants(UserProfile.TenantId!)
            .ToDictionary(k => k.Id, k => k.Name);

        var cached = await SessionStorage.GetAsync<ActivityPQASessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            Query.Keyword = sd.Keyword;
            Query.OrderBy = sd.OrderBy;
            Query.PageNumber = sd.PageNumber;
            Query.PageSize = 50;
            Query.SortDirection = sd.SortDirection;
            Query.SupportWorkerId = sd.SupportWorkerId;
            Query.TenantId = sd.TenantId;
            Query.ActivityTypeId = sd.ActivityTypeId;
        }

        await OnRefresh();
    }

    private void OnRowClick(TableRowClickEventArgs<ActivityQueueEntryDto> args)
    {
        if (args.Item is not null)
        {
            Navigation.NavigateTo($"/pages/workspace/deliverymanagement/activities/pqa/{args.Item.Id}");
        }
    }

    private Task PageChanged(int page)
    {
        Query.PageNumber = page;
        return OnRefresh();
    }

    private async Task ClearSearch()
    {
        ResetQuery();
        await OnRefresh();
    }

    private void ResetQuery()
    {
        Query.SupportWorkerId = null;
        Query.TenantId = null;
        Query.ActivityTypeId = null;
        Query.OrderBy = "CommencedOn";
        Query.SortDirection = nameof(SortDirection.Ascending);
        Query.PageNumber = 1;
        Query.PageSize = 50;
        Query.Keyword = null;
    }

    private Task OnSearch(string? text)
    {
        Query.Keyword = text;
        return OnRefresh();
    }

    private Task OnRefreshClicked()
        => OnRefresh();

    private async Task OnRefresh()
    {
        try
        {
            _loading = true;
            Query.CurrentUser = UserProfile;

            var signature = BuildUsersFilterSignature();
            if (signature != _usersFilterSignature)
            {
                await LoadUsersAsync();
                _usersFilterSignature = signature;
            }

            var results = await Service.Send(Query);
            if (results is { Succeeded: true, Data: not null })
            {
                _data = results.Data.Items.ToArray();
                _totalPages = results.Data.TotalPages;
                _totalItems = results.Data.TotalItems;
            }
            else
            {
                _data = [];
                _totalPages = 0;
                _totalItems = 0;
            }
            await SessionStorage.SetAsync(ActivityPQASessionData.FromQuery(Query));
        }
        finally
        {
            _loading = false;
        }
    }

    private string BuildUsersFilterSignature()
        => string.Join('|', Query.Keyword, Query.TenantId, Query.ActivityTypeId);

    private async Task LoadUsersAsync()
    {
        var assigneesResult = await Service.Send(new GetActivityPqaAssignees.Query(UserProfile)
        {
            Keyword = Query.Keyword,
            TenantId = Query.TenantId,
            ActivityTypeId = Query.ActivityTypeId
        });

        _users = assigneesResult is { Succeeded: true, Data: not null }
            ? assigneesResult.Data.ToDictionary(a => a.Id, e => e.DisplayName)
            : new Dictionary<string, string>();
    }

    private async Task ActivityTypeChanged(int? activityTypeId)
    {
        Query.ActivityTypeId = activityTypeId;
        await OnRefresh();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportPqaActivities.Command()
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
            { "CurrentUser", UserProfile }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select Tenant", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            Query.TenantId = tenant.TenantId;
            Query.SupportWorkerId = null;
            await OnRefresh();
        }
    }

    private async Task ShowSupportWorkerDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", UserProfile },
            { "Filter", (Func<ApplicationUserDto, bool>)(u => _users.ContainsKey(u.Id)) }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select Support Worker", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedUser user })
        {
            Query.SupportWorkerId = user.UserId;
            await OnRefresh();
        }
    }

    private async Task SortBy(string key)
    {
        if (Query.OrderBy == key)
        {
            Query.SortDirection = Query.SortDirection == nameof(SortDirection.Ascending)
                ? nameof(SortDirection.Descending)
                : nameof(SortDirection.Ascending);
        }
        else
        {
            Query.OrderBy = key;
            Query.SortDirection = nameof(SortDirection.Ascending);
        }

        await OnRefresh();
    }
}
