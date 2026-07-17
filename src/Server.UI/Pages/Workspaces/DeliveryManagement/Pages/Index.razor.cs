using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Index
{
    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];
    
    private bool _showCaseWorkload;
    private bool _showUnassignedCases;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;

        var hasAdditionalRoles = (await AuthService.AuthorizeAsync(authState.User, SecurityPolicies.UserHasAdditionalRoles)).Succeeded;
        var canViewCumulatives = authState.User.IsInRole(RoleNames.SystemSupport) || authState.User.IsInRole(RoleNames.Finance);

        var canViewPqa = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Pqa)).Succeeded;
        var canViewPayments = authState.User.IsInRole(RoleNames.SystemSupport) || authState.User.IsInRole(RoleNames.Finance);

        _showCaseWorkload = hasAdditionalRoles;
        _showUnassignedCases = hasAdditionalRoles;

        // Case Management and Performance are available to every authorised user: support
        // workers see their own data, senior staff can drill down by tenant and user.
        List<BreadcrumbLinkModel> links =
        [
            DeliveryManagementLinks.Performance,
            DeliveryManagementLinks.LocationBreakdown,
            DeliveryManagementLinks.PathwayPlanReviews,
            DeliveryManagementLinks.Initiatives,
            DeliveryManagementLinks.RecentApprovedActivities,
        ];

        if (_showCaseWorkload)
        {
            links.Add(DeliveryManagementLinks.CaseWorkload);
        }

        if (_showUnassignedCases)
        {
            links.Add(DeliveryManagementLinks.UnassignedCases);
        }

        if (canViewCumulatives)
        {
            links.Add(DeliveryManagementLinks.Cumulatives);
        }

        if (canViewPqa)
        {
            links.Add(DeliveryManagementLinks.EnrolmentsPqa);
            links.Add(DeliveryManagementLinks.ActivitiesPqa);
        }

        if (canViewPayments)
        {
            links.Add(DeliveryManagementLinks.Payments);
        }

        Links = links.ToArray();        

    }

}