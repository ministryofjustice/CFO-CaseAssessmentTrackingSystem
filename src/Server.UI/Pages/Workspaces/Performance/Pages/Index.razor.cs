using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
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

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var allowOutcomeQualityDipChecks = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.OutcomeQualityDipChecks)).Succeeded;

        List<BreadcrumbLinkModel> links = [];

        links.Add(PerformanceLinks.OutcomeQualityDipSamples);
        links.Add(PerformanceLinks.ArchivedCaseBehaviour);

        Links = links.ToArray();

    }
}
