using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class TasksDue
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    [Inject]
    public IUserService UserService { get; set; } = null!;

    [Inject]
    public ITenantService TenantService { get; set; } = null!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    private TasksDueWithPagination.TaskDueDto[] _data = [];
    private int _totalPages;
    private int _totalItems;

    private IDictionary<string, string> _users = new Dictionary<string, string>();
    private IDictionary<string, string> _tenants = new Dictionary<string, string>();

    private bool Tabular { get; set; }

    private TasksDueWithPagination.Query Query { get; set; } = new()
    {
        CurrentUser = null!,
        PageNumber = 1,
        PageSize = 15,
        OrderBy = "Due",
        SortDirection = "Ascending"
    };

    protected override async Task OnInitializedAsync()
    {
        Query.CurrentUser = UserProfile;

        _users = UserService.DataSource
            .Where(d => d.TenantId!.StartsWith(UserProfile.TenantId!))
            .ToDictionary(a => a.Id, e => e.DisplayName);

        _tenants = TenantService.GetVisibleTenants(UserProfile.TenantId!)
            .ToDictionary(k => k.Id, k => k.Name);

        if (UserProfile.AssignedRoles is [])
        {
            Query.OwnerId = UserProfile.UserId;
        }

        var cached = await SessionStorage.GetAsync<TasksDueSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            Query.OwnerId = sd.OwnerId;
            Query.TenantId = sd.TenantId;
            Query.Category = sd.Category;
            Query.Keyword = sd.Keyword;
            Query.OrderBy = sd.OrderBy ?? "Due";
            Query.SortDirection = sd.SortDirection ?? "Ascending";
            Query.PageNumber = sd.PageNumber;
            Tabular = sd.Tabular;
        }

        await OnRefresh();
    }

    private async Task OnRefresh()
    {
        Query.CurrentUser = UserProfile;
        var result = await GetNewMediator().Send(Query);

        if (result is { Succeeded: true, Data: not null })
        {
            _data = result.Data.Items.ToArray();
            _totalPages = result.Data.TotalPages;
            _totalItems = result.Data.TotalItems;
        }
        else
        {
            _data = [];
            _totalPages = 0;
            _totalItems = 0;
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }

        await SessionStorage.SetAsync(TasksDueSessionData.FromQuery(Query, Tabular));
    }

    private async Task TabularChanged(bool? tabular)
    {
        Tabular = tabular.GetValueOrDefault();
        await SessionStorage.SetAsync(TasksDueSessionData.FromQuery(Query, Tabular));
    }

    private async Task PageChanged(int page)
    {
        Query.PageNumber = page;
        await OnRefresh();
    }

    private async Task OnSearch(string? keyword)
    {
        Query.Keyword = keyword;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task SortBy(string key)
    {
        if (Query.OrderBy == key)
        {
            Query.SortDirection = Query.SortDirection == "Ascending" ? "Descending" : "Ascending";
        }
        else
        {
            Query.OrderBy = key;
            Query.SortDirection = "Ascending";
        }
        await OnRefresh();
    }

    private async Task ApplyCategoryFilter(TaskDueCategory? category)
    {
        Query.Category = category;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private string GetCurrentFilterLabel()
        => Query.Category switch
        {
            TaskDueCategory.Overdue => "Overdue",
            TaskDueCategory.DueImminently => "Due Imminently (Next Week)",
            TaskDueCategory.DueSoon => "Due Soon (Next Month)",
            _ => "All Outstanding Tasks"
        };

    private static string CategoryLabel(TaskDueCategory category)
        => category switch
        {
            TaskDueCategory.Overdue => "Overdue",
            TaskDueCategory.DueImminently => "Due Imminently",
            _ => "Due Soon"
        };

    private static Color CategoryColour(TaskDueCategory category)
        => category switch
        {
            TaskDueCategory.Overdue => Color.Error,
            TaskDueCategory.DueImminently => Color.Warning,
            _ => Color.Info
        };

    private async Task ShowOwnerDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog> { { "CurrentUser", UserProfile }, { "ShowAllOption", !string.IsNullOrEmpty(Query.OwnerId) } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select a Support Worker", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedUser user })
        {
            Query.OwnerId = string.IsNullOrEmpty(user.UserId) ? null : user.UserId;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ShowTenantDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog> { { "CurrentUser", UserProfile }, { "ShowAllOption", !string.IsNullOrEmpty(Query.TenantId) } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select a tenant", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            Query.TenantId = string.IsNullOrEmpty(tenant.TenantId) ? null : tenant.TenantId;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ClearSearch()
    {
        Query.OwnerId = UserProfile.AssignedRoles is [] ? UserProfile.UserId : null;
        Query.TenantId = null;
        Query.Category = null;
        Query.Keyword = null;
        Query.OrderBy = "Due";
        Query.SortDirection = "Ascending";
        Query.PageNumber = 1;
        Tabular = false;
        await OnRefresh();
    }

    private void ViewParticipant(TasksDueWithPagination.TaskDueDto task)
        => Navigation.NavigateTo($"/pages/workspace/participants/{task.ParticipantId}?from=tasks-due");
}
