using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Announcements;

public partial class Announcements
{
    [SupplyParameterFromQuery(Name = "internal")] public bool Internal { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    [CascadingParameter] private UserProfile CurrentUser { get; set; } = null!;

    private bool _loading = true;
    private bool _canViewInternal;
    private bool _canManage;
    private bool _canPublishImmediately;
    private int _activeTab;
    private readonly IReadOnlyList<BreadcrumbItem> _breadcrumbs =
    [
        new("Announcements", href: null, disabled: true)
    ];

    protected override async Task OnParametersSetAsync()
    {
        var state = await AuthState;
        _canViewInternal = CurrentUser.IsInternalUser;
        _canManage = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SeniorInternal)).Succeeded;
        _canPublishImmediately = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions)).Succeeded;
        _activeTab = Internal && _canViewInternal ? 1 : 0;
        _loading = false;
    }
}
