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
        var canViewCumulatives = authState.User.IsInRole(RoleNames.SystemSupport) || authState.User.IsInRole(RoleNames.Finance);

        var canViewPqa = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Pqa)).Succeeded;
        var canViewPayments = authState.User.IsInRole(RoleNames.SystemSupport) || authState.User.IsInRole(RoleNames.Finance);

        _showCaseWorkload = hasAdditionalRoles;

        // Case Management and Performance are available to every authorised user: support
        // workers see their own data, senior staff can drill down by tenant and user.
        List<BreadcrumbLinkModel> links =
        [
            ProviderLinks.CaseManagement,
            ProviderLinks.Performance,
            ProviderLinks.RecentApprovedActivities,
        ];

        if (_showCaseWorkload)
        {
            links.Add(ProviderLinks.CaseWorkload);
        }

        if (canViewCumulatives)
        {
            links.Add(ProviderLinks.Cumulatives);
        }

        if (canViewPqa)
        {
            links.Add(ProviderLinks.EnrolmentsPqa);
            links.Add(ProviderLinks.ActivitiesPqa);
        }

        if (canViewPayments)
        {
            links.Add(ProviderLinks.Payments);
        }

        Links = links.ToArray();        

    }

}