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
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];    

protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        var isInternalUser = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Internal)).Succeeded;

        List<BreadcrumbLinkModel> links = [];

        //links.Add(ServiceDeskLinks.Home);
        links.Add(ServiceDeskLinks.ActivitiesQueue);
        links.Add(ServiceDeskLinks.ActivitiesFeedback);
        links.Add(ServiceDeskLinks.EnrolmentsQueue);
        links.Add(ServiceDeskLinks.EnrolmentsFeedback);
        if (isInternalUser)
        {
            links.Add(ServiceDeskLinks.SyncInfo);
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