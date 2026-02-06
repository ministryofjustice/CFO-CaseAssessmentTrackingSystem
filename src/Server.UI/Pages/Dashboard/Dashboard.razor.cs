using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

[Authorize(Policy = SecurityPolicies.AuthorizedUser)]
public partial class Dashboard
{
    private bool _showQaPots;
    private bool _showMyTeamsParticipants;
    private bool _showJobManagement;
    private bool _showCaseWorkload;
    private bool _showRiskDueAggregate;
    private bool _showSyncComponent;
    private bool _showSearchParticipant;

    private readonly string _title  = "Dashboard";

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        // Check user roles/permissions once
        var hasAdditionalRoles = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.UserHasAdditionalRoles)).Succeeded;
        var isInternalUser = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;
        var canAccessSystemSupport = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions)).Succeeded;

        // Feature flags derived from permissions
        _showMyTeamsParticipants = hasAdditionalRoles;
        _showCaseWorkload = hasAdditionalRoles;
        _showRiskDueAggregate = hasAdditionalRoles;
        
        _showQaPots = isInternalUser;
        _showSearchParticipant = isInternalUser;
        _showSyncComponent = isInternalUser;

        _showJobManagement = canAccessSystemSupport;
    }  
}