using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages;

public partial class Index
{
    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];    
    
    private bool _showQaPots;
    private bool _showSyncInfo;
    private bool _showActivitiesQueue;
    private bool _showActivitiesFeedback;
    private bool _showEnrolmentsQueue;
    private bool _showEnrolmentsFeedback;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var isInternalUser = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Internal))
            .Succeeded;
        var isQa1User = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Qa1))
            .Succeeded;
        var canAccessSyncInfo = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.ServiceDeskSyncInformation))
            .Succeeded;
        var canViewQueueLinks = isQa1User && CurrentUser.AssignedRoles.Any(role =>
            role is RoleNames.QASupportManager or RoleNames.QAManager or RoleNames.SMT or RoleNames.SystemSupport);

        _showQaPots = isInternalUser;
        _showSyncInfo = canAccessSyncInfo;
        _showActivitiesQueue = canViewQueueLinks;
        _showActivitiesFeedback = isQa1User;
        _showEnrolmentsQueue = canViewQueueLinks;
        _showEnrolmentsFeedback = isQa1User;
        
        List<BreadcrumbLinkModel> links = [];

        if(_showActivitiesQueue)
        {
            links.Add(ServiceDeskLinks.ActivitiesQueue);
        }
        
        if(_showActivitiesFeedback)
        {  
            links.Add(ServiceDeskLinks.ActivitiesFeedback);
        }

        if(_showEnrolmentsQueue)
        {
            links.Add(ServiceDeskLinks.EnrolmentsQueue);
        }
        
        if(_showEnrolmentsFeedback)
        {
            links.Add(ServiceDeskLinks.EnrolmentsFeedback);
        }
        
        if (_showSyncInfo)
        {
            links.Add(ServiceDeskLinks.SyncInfo);
        }

        if (_showQaPots)
        {
            links.Add(ServiceDeskLinks.QaPots);
        }

        Links = links.ToArray();
    }

    private Task OnViewQueueNavigate(string target)
    {
        var destination = target switch
        {
            "Activities|First Pass" => $"{ServiceDeskLinks.ActivitiesQueue.Href}?tab=first-pass",
            "Activities|Second Pass" => $"{ServiceDeskLinks.ActivitiesQueue.Href}?tab=second-pass",
            "Activities|Escalation" => $"{ServiceDeskLinks.ActivitiesQueue.Href}?tab=escalation",
            "Enrolments|First Pass" => $"{ServiceDeskLinks.EnrolmentsQueue.Href}?tab=first-pass",
            "Enrolments|Second Pass" => $"{ServiceDeskLinks.EnrolmentsQueue.Href}?tab=second-pass",
            "Enrolments|Escalation" => $"{ServiceDeskLinks.EnrolmentsQueue.Href}?tab=escalation",
            _ => ServiceDeskLinks.Home.Href
        };

        Navigation.NavigateTo(destination);
        return Task.CompletedTask;
    }
}
