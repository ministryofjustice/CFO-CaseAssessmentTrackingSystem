using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components;

/// <summary>
/// Shared base for the Provider workspace dashboard pages (Case Management and Performance).
/// Encapsulates the senior-staff drill-down filtering so the pages only differ in the tabs they render.
/// </summary>
public abstract class ProviderDashboardBase : CatsComponentBase
{
    [Inject]
    protected IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [CascadingParameter]
    protected UserProfile CurrentUser { get; set; } = null!;

    /// <summary>
    /// True for staff who may filter the dashboard by tenant and user. Support workers cannot
    /// filter and are scoped to their own data.
    /// </summary>
    protected bool CanFilter { get; private set; }

    protected string? SelectedTenantId { get; private set; }

    protected string? SelectedUserId { get; private set; }

    protected bool VisualMode { get; set; } = true;

    /// <summary>
    /// Tenant actually queried by the child dashboards. When nothing is selected we fall back to the
    /// signed-in user's own tenant, so staff see their tenant's data without having to choose first.
    /// A drilled-in user takes precedence, so we drop the tenant filter in that case.
    /// </summary>
    protected string? EffectiveTenantId =>
        string.IsNullOrWhiteSpace(SelectedTenantId) is false ? SelectedTenantId
        : string.IsNullOrWhiteSpace(SelectedUserId) is false ? null
        : CurrentUser.TenantId;

    /// <summary>
    /// User actually queried by the child dashboards. Support workers are pinned to themselves;
    /// senior staff only set this when they drill into a specific user.
    /// </summary>
    protected string? EffectiveUserId => SelectedUserId;

    /// <summary>
    /// True when viewing a whole tenant (no specific user drilled into). Used to gate
    /// tenant-only components such as Unassigned Cases.
    /// </summary>
    protected bool IsTenantLevel =>
        string.IsNullOrWhiteSpace(EffectiveUserId)
        && string.IsNullOrWhiteSpace(EffectiveTenantId) is false;

    /// <summary>
    /// Composite key used to force child dashboard components to refresh when the selection changes.
    /// </summary>
    protected string SelectionKey => $"{EffectiveUserId}|{EffectiveTenantId}";

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        CanFilter = (await AuthorizationService.AuthorizeAsync(state.User, SecurityPolicies.UserHasAdditionalRoles)).Succeeded;

        if (CanFilter is false)
        {
            // Support workers are locked to their own data.
            SelectedUserId = CurrentUser.UserId;
        }

        // Staff who can filter start unfiltered ("All Tenants"), mirroring the Participants list,
        // and pick a tenant or drill into a specific user from the header.
    }

    /// <summary>
    /// Applies a tenant selection from the drill-down header. Changing the tenant drills back up,
    /// clearing any user we had drilled into.
    /// </summary>
    protected void OnTenantSelected(string? tenantId)
    {
        SelectedTenantId = tenantId;
        SelectedUserId = null;
    }

    /// <summary>
    /// Drills into a specific user within the currently selected tenant.
    /// </summary>
    protected void OnUserSelected(string? userId) => SelectedUserId = userId;

    /// <summary>
    /// Clears the drill-down back to All Tenants / All Users, mirroring the Participants list.
    /// </summary>
    protected void OnClearFilter()
    {
        SelectedTenantId = null;
        SelectedUserId = null;
    }
}
