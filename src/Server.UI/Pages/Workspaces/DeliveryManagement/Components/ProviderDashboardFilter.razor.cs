using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components;

/// <summary>
/// Tenant + user drill-down selectors for the Provider workspace dashboards, following the same
/// dialog-driven pattern as the Participants list. Rendered only for staff who may filter. Each
/// calling page supplies its own tenant-scoped <see cref="Users"/> candidate list, computed via a
/// dedicated query matching that page's own underlying data (e.g. participant owners vs. activity
/// owners vs. payment support workers) — this component does not derive the list itself, since a
/// single "all tenant users" or "one generic query" doesn't fit every dashboard.
/// </summary>
public partial class ProviderDashboardFilter
{
    [Inject]
    private ITenantService TenantService { get; set; } = null!;

    [Parameter, EditorRequired]
    public UserProfile CurrentUser { get; set; } = null!;

    [Parameter, EditorRequired]
    public IDictionary<string, string> Users { get; set; } = new Dictionary<string, string>();

    [Parameter]
    public string? SelectedTenantId { get; set; }

    [Parameter]
    public string? SelectedUserId { get; set; }

    [Parameter]
    public EventCallback<string?> TenantSelected { get; set; }

    [Parameter]
    public EventCallback<string?> UserSelected { get; set; }

    [Parameter]
    public EventCallback ClearRequested { get; set; }

    [Parameter]
    public bool ShowUser { get; set; }= true;

    /// <summary>
    /// Label shown when no user is selected. Defaults to "All Users", but pages whose picker
    /// lists something other than users (e.g. Initiatives' assignees) can override it.
    /// </summary>
    [Parameter]
    public string AllUsersLabel { get; set; } = "All Users";

    private IDictionary<string, string> _tenants = new Dictionary<string, string>();

    private string TenantLabel =>
        string.IsNullOrEmpty(SelectedTenantId)
            ? "All Tenants"
            : _tenants.TryGetValue(SelectedTenantId, out var name) ? name : SelectedTenantId;

    private string UserLabel =>
        string.IsNullOrEmpty(SelectedUserId)
            ? AllUsersLabel
            : Users.TryGetValue(SelectedUserId, out var name) ? name : SelectedUserId;

    protected override void OnInitialized() =>
        _tenants = TenantService.GetVisibleTenants(CurrentUser.TenantId!)
            .ToDictionary(k => k.Id, k => k.Name);

    private async Task ShowTenantDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", CurrentUser }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select a tenant", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            await TenantSelected.InvokeAsync(string.IsNullOrEmpty(tenant.TenantId) ? null : tenant.TenantId);
        }
    }

    private async Task ShowUserDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", GetEffectiveUserProfile() },
            { "Filter", (Func<ApplicationUserDto, bool>)(u => Users.ContainsKey(u.Id)) }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select a user", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedUser user })
        {
            await UserSelected.InvokeAsync(string.IsNullOrEmpty(user.UserId) ? null : user.UserId);
        }
    }

    private Task ClearFilter() => ClearRequested.InvokeAsync();

    /// <summary>
    /// Returns a profile scoped to the selected tenant so the user picker only lists users within
    /// the tenant the senior has drilled into. Falls back to the current user for their own tenant.
    /// </summary>
    private UserProfile GetEffectiveUserProfile()
    {
        if (string.IsNullOrEmpty(SelectedTenantId) || SelectedTenantId == CurrentUser.TenantId)
        {
            return CurrentUser;
        }

        return new UserProfile
        {
            UserId = CurrentUser.UserId,
            UserName = CurrentUser.UserName,
            Email = CurrentUser.Email,
            DisplayName = CurrentUser.DisplayName,
            PhoneNumber = CurrentUser.PhoneNumber,
            TenantId = SelectedTenantId,
            TenantName = CurrentUser.TenantName,
            AssignedRoles = CurrentUser.AssignedRoles,
            DefaultRole = CurrentUser.DefaultRole,
            Contracts = CurrentUser.Contracts,
            Status = CurrentUser.Status,
            Provider = CurrentUser.Provider,
            SuperiorName = CurrentUser.SuperiorName,
            SuperiorId = CurrentUser.SuperiorId,
            ProfilePictureDataUrl = CurrentUser.ProfilePictureDataUrl
        };
    }
}
