using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

[Authorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
public partial class QaDashboard
{
    private bool _showQaPots;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _showQaPots = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;
        await base.OnInitializedAsync();
    }

}