using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages;

public partial class Index
{
    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];
    
    private bool _showCaseWorkload;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;

        var hasAdditionalRoles = (await AuthService.AuthorizeAsync(authState.User, SecurityPolicies.UserHasAdditionalRoles)).Succeeded;
        
        _showCaseWorkload = hasAdditionalRoles;

        List<BreadcrumbLinkModel> links = [];

        if (_showCaseWorkload)
        {
            links.Add(ProviderLinks.CaseWorkload);
        }

        Links = links.ToArray();        

    }

}