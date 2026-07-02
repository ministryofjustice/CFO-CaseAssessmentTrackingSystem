using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

[Authorize(Policy = SecurityPolicies.Internal)]
public partial class Index
{
    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    private bool _showJobManagement;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        var canAccessSystemSupport =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions)).Succeeded;

        _showJobManagement = canAccessSystemSupport;

        List<BreadcrumbLinkModel> links = [];

        if (_showJobManagement)
        {
            links.Add(AdministrationLinks.Jobs);
            links.Add(AdministrationLinks.CacheManagement);
        }

        links.Add(AdministrationLinks.AuditTrails);
        
        Links = links.ToArray();
    }
}
