using System.Security.Claims;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public class AsyncMenuService(IAuthorizationService authorizationService) : IAsyncMenuService
{
    public async Task<NavigationMenuModel> GetFeaturesAsync(ClaimsPrincipal principal)
    {
        if(principal.Identity?.IsAuthenticated is false)
        {
            return new NavigationMenuModel([]);
        }

        NavigationMenuSectionModel[] sections = [
            await CreateWorkspaceMenu(principal),
            await CreateExternalLinksMenu(),
            await CreateProfileMenu(),            
        ];     

        return new NavigationMenuModel(sections);
    }

    private async Task<NavigationMenuSectionModel> CreateWorkspaceMenu(ClaimsPrincipal principal)
    {
        List<NavigationMenuItemLinkModel> items =
        [
            new NavigationMenuItemLinkModel("Participants", "/pages/workspace/participants", "Navigates to the root workspace for accessing participant management functions"),
            new NavigationMenuItemLinkModel("Provider", "/pages/workspace/deliverymanagement", "Navigates to the root workspace for accessing delivery management information and functions"),
        ];

        if(await PassesPolicy( principal, SecurityPolicies.ServiceDesk))
        {
            items.Add(new NavigationMenuItemLinkModel("Service Desk", "/pages/workspace/servicedesk", "Navigates to the root workspace for accessing service desk functions"));
        }

        if(await PassesPolicy(principal, SecurityPolicies.OutcomeQualityDipChecks))
        {
            items.Add(new NavigationMenuItemLinkModel("Performance Management", "/pages/workspace/performance", "Navigates to the root workspace for accessing performance function"));    
        }

        if(await PassesPolicy(principal, SecurityPolicies.SystemFunctionsRead))
        {
            items.Add(new NavigationMenuItemLinkModel("Administration", "/pages/workspace/administration", "Navigates to the root workspace for CATS administrative function"));
        }
        
        return new("Workspaces", items.ToArray(), IsBookmarkable: true);
    }

    private async Task<NavigationMenuSectionModel> CreateProfileMenu() => new("Profile", [
        new NavigationMenuItemLinkModel("My Account", "/pages/workspace/account", "Manage my account, documents and notifications"),
        new NavigationMenuItemDividerModel(),
        new NavigationMenuItemLinkModel("Logout", "/pages/authentication/logout", "Logs out of the system"),
    ]);

    private async Task<NavigationMenuSectionModel> CreateExternalLinksMenu() => new("External Links", [
        new NavigationMenuItemLinkModel("CFO Website", "https://www.creatingfutureopportunities.gov.uk/", "Navigates to the CFO website on a new tab", "_blank"),
        new NavigationMenuItemLinkModel("CFO Maps", "https://www.creatingfutureopportunities.gov.uk/map/", "Navigates to the CFO Maps website on a new tab", "_blank"),
        new NavigationMenuItemLinkModel("GitHub", "https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/", "Navigates to the GitHub repository on a new tab", "_blank")
    ]);
    
    private async Task<bool> PassesPolicy(ClaimsPrincipal principal, string policy)
    {
        var result = await authorizationService.AuthorizeAsync(principal, policy);
        return result.Succeeded;
    }
}