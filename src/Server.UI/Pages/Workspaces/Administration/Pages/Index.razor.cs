using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

[Authorize(Policy = SecurityPolicies.AuthorizedUser)]
public partial class Index
{
    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];
    
    private bool _showSyncComponent;
    private bool _showJobManagement;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        var canAccessSystemSupport =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions)).Succeeded;
        var isInternalUser = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;
      
        _showSyncComponent = isInternalUser;
        _showJobManagement = canAccessSystemSupport;

        List<BreadcrumbLinkModel> links = [];

        if (_showJobManagement)
        {
            links.Add(AdministrationLinks.Jobs);
            links.Add(AdministrationLinks.CacheManagement);
        }

        if (_showSyncComponent)
        {
            links.Add(AdministrationLinks.SyncInformation);
        }

        Links = links.ToArray();
    }
}
