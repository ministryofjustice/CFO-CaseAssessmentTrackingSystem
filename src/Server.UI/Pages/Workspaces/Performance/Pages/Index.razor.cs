using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Pages;

public partial class Index
{
    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    private bool _allowOutcomeQualityDipChecks = false;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        _allowOutcomeQualityDipChecks = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.OutcomeQualityDipChecks)).Succeeded;
    }

    private IReadOnlyList<BreadcrumbItem> BreadcrumbItems = [
        new BreadcrumbItem(PerformanceLinks.Home.Title, PerformanceLinks.Home.Url),
    ];
}
