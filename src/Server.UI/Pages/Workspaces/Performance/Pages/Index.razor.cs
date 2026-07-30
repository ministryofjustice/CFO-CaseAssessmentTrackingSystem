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
    public Task<AuthenticationState> AuthState { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];
    
    private bool _showOutcomeQualityDipChecks;
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        
        var isOutcomeQualityDipChecks = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.OutcomeQualityDipChecks)).Succeeded;
      
        _showOutcomeQualityDipChecks = isOutcomeQualityDipChecks;
        
        List<BreadcrumbLinkModel> links = [];

        if (_showOutcomeQualityDipChecks)
        {
            links.Add(PerformanceLinks.OutcomeQualityDipSamples);
        }
        
        Links = links.ToArray();
    }
}
