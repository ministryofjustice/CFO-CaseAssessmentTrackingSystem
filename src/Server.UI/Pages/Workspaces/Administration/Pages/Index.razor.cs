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
    private bool _showSystemFunctions;
    private bool _showServiceDeskManagement;
    private bool _showSeniorInternal;
      
    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        var canAccessSystemSupport =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions)).Succeeded;
        var canAccessSystemFunctions =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemFunctionsRead)).Succeeded;
        var canAccessServiceDeskManagement =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.ServiceDeskManagement)).Succeeded;
        var canAccessSeniorInternal =
            (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SeniorInternal)).Succeeded;

        _showJobManagement = canAccessSystemSupport;
        _showSystemFunctions = canAccessSystemFunctions;
        _showServiceDeskManagement = canAccessServiceDeskManagement;
        _showSeniorInternal = canAccessSeniorInternal;
            
        List<BreadcrumbLinkModel> links = [];

        if (_showJobManagement)
        {
            links.Add(AdministrationLinks.Jobs);
            links.Add(AdministrationLinks.CacheManagement);
        }
        
        if(_showServiceDeskManagement)
        {
            links.Add(AdministrationLinks.AuditTrails);
            links.Add(AdministrationLinks.Outbox);
        }
        
        if (_showSystemFunctions)
        {
            links.Add(AdministrationLinks.PickList);
        }
        
        if (_showSeniorInternal)
        {
            links.Add(AdministrationLinks.Labels);
        }
        
        Links = links.ToArray();
    }
}
