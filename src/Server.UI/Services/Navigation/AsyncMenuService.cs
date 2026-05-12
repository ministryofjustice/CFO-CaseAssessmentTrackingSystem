using System.Configuration;
using System.Security.Claims;
using Ardalis.Specification;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Infrastructure.Constants.ClaimTypes;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public class AsyncMenuService(IAuthorizationService authorizationService) : IAsyncMenuService
{
    public async Task<IEnumerable<MenuSectionModel>> GetFeaturesAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated == false)
        {
            return [];
        }

        List<MenuSectionModel> menuItems = [];

        var application = await CreateApplicationMenu(principal);

        menuItems.Add(application);

        if (await PassesPolicy(principal, SecurityPolicies.UserHasAdditionalRoles))
        {
            MenuSectionModel qaMenu = await CreateQaMenu(principal);

            if (qaMenu.SectionItems!.Any(s => s.MenuItems!.Count() > 0))
            {
                // only add the menu if at least on sub menu has been added.
                menuItems.Add(qaMenu);
            }
        }

        if (await PassesPolicy(principal, SecurityPolicies.Finance))
        {
            menuItems.Add(new()
            {
               Title = "Finance",
               SectionItems = [
                   new(){
                       Title = "Payments",
                       Icon = Icons.Material.Filled.Money,
                       Href="/pages/finance/payments"
                   }
               ] 
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.Internal))
        {
            MenuSectionModel management = new()
            {
                Title = "Management"
            };
            
            if(await PassesPolicy(principal, SecurityPolicies.UserManagement))
            {
                management.SectionItems.Add(new()
                {
                   IsParent = true,
                   Title = "Authorization",
                   Icon = Icons.Material.Filled.ManageAccounts,
                   MenuItems = [
                        new()
                        {
                            Title = "Tenants",
                            Href = "/administration/tenants"
                        },

                        new()
                        {
                            Title = "Users",
                            Href = "/identity/users"
                        },
                        new()
                        {
                            Title = "User Audit",
                            Href = "/identity/users/audit"
                        },

                        new()
                        {
                            Title = "Profile",
                            Href = "/user/profile"
                        }
                   ] 
                });

                if(await PassesPolicy(principal, SecurityPolicies.ServiceDeskManagement))
                {
                    management.SectionItems.Add(new()
                    {
                       IsParent = true,
                       Title = "System",
                       Icon = Icons.Material.Filled.Devices,
                       MenuItems = [
                            new()
                            {
                                Title = "Lookup Values",
                                Href = "/system/picklist"
                            },
                            new()
                            {
                                Title = "Audit Trails",
                                Href = "/system/audittrails",
                            },
                            new()
                            {
                                Title = "Outbox Messages",
                                Href = "/system/outbox"
                            },
                            new MenuSectionSubItemModel()
                            {
                                Title = "Labels",
                                Href = "/pages/labels",
                                PageStatus = PageStatus.Wip
                            }
                       ] 
                    });
                }

                menuItems.Add(management);
            }

            if(await PassesPolicy(principal, SecurityPolicies.Initiatives))
            {
                management.SectionItems.Add(new()
                {
                   IsParent = false,
                   Title = "Initiatives",
                   Icon = Icons.Material.Filled.Lightbulb,
                   Href = "/pages/initiatives",
                   PageStatus = PageStatus.Wip
                });
            }

        }

        return menuItems;
    }

    private async Task<MenuSectionModel> CreateApplicationMenu(ClaimsPrincipal principal) => new()
    {
        Title = "Application",
        SectionItems =
                [
                    await CreateDashboardMenu(principal),
                    await CreateParticipantsMenu(principal),
                    await CreateAnalyticsMenu(principal)
                ]
    };

    private async Task<MenuSectionModel> CreateQaMenu(ClaimsPrincipal principal)
    {
        MenuSectionModel qa = new()
        {
            Title="Quality Control",
        };

        MenuSectionItemModel enrolments = new()
        {
           IsParent = true,
           Title = "Enrolments",
           Icon = Icons.Material.Filled.Approval,
        };

        if(await PassesPolicy(principal, SecurityPolicies.Pqa))
        {
            enrolments.MenuItems.Add(new()
            {
                Title = "PQA",
                Href = "/pages/qa/enrolments/pqa",
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.ServiceDeskManagement))
        {
            enrolments.MenuItems.Add(new()
            {
               Title = "Queue Management",
               Href = "/pages/qa/servicedesk/enrolments" 
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.Qa1))
        {
            enrolments.MenuItems.Add(new()
            {
                Title = "First Pass",
                Href = "/pages/qa/enrolments/qa1/",
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.Qa2))
        {
            enrolments.MenuItems.Add(new()
            {
                Title = "Second Pass",
                Href = "/pages/qa/enrolments/qa2/",
            });
        }

        qa.SectionItems.Add(enrolments);

        MenuSectionItemModel activities = new()
        {
            IsParent = true,
            Title = "Activities",
            Icon = Icons.Material.Filled.SelfImprovement
        };

        if(await PassesPolicy(principal, SecurityPolicies.Pqa))
        {
            activities.MenuItems.Add(new()
            {
                Title = "PQA",
                Href = "/pages/qa/activities/pqa",
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.ServiceDeskManagement))
        {
            activities.MenuItems.Add(new()
            {
               Title = "Queue Management",
               Href = "/pages/qa/activities/activities" 
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.Qa1))
        {
            activities.MenuItems.Add(new()
            {
                Title = "First Pass",
                Href = "/pages/qa/activities/qa1/",
            });
        }

        if(await PassesPolicy(principal, SecurityPolicies.Qa2))
        {
            activities.MenuItems.Add(new()
            {
                Title = "Second Pass",
                Href = "/pages/qa/activities/qa2/",
            });
        }

        qa.SectionItems.Add(activities);

        return qa;
    }

    private async Task<MenuSectionItemModel> CreateAnalyticsMenu(ClaimsPrincipal principal)
    {
        MenuSectionItemModel analytics = new()
        {
            Title = "Analytics",
            Icon = Icons.Material.Filled.Analytics,
            IsParent = true,
            MenuItems = []
        };

        if (await PassesPolicy(principal, SecurityPolicies.Finance))
        {
            analytics.MenuItems.Add(new()
            {
                Title = "Cumulatives",
                Href = "/pages/analytics/cumulatives",
            });
        }

        analytics.MenuItems.Add(new()
        {
            Title = "My Documents",
            Href = "/pages/analytics/my-documents",
        });

        if(await PassesPolicy(principal, SecurityPolicies.OutcomeQualityDipChecks))
        {
            analytics.MenuItems.Add(new()
            {
                Title = "Outcome Quality",
                Href = "/pages/analytics/outcome-quality-dip-sampling",
            });
        }
        
        return analytics;
    }

    private async Task<MenuSectionItemModel> CreateParticipantsMenu(ClaimsPrincipal principal)
    {
        // participants
        MenuSectionItemModel participants = new()
        {
            Title = "Participants",
            Icon = Icons.Material.Filled.EmojiPeople,
            IsParent = true,
            MenuItems =
            [
                new()
                {
                    Title = "All",
                    Href = "/pages/participants",
                },
                new()
                {
                    Title = "All (Version 2)",
                    Href = "/pages/participantsV2",
                },
                new()
                {
                    Title = "Moved Participants",
                    Href = "/pages/participants/movedparticipants",
                },
            ]
        };

        if (await PassesPolicy(principal, SecurityPolicies.Reassign))
        {
            participants.MenuItems.Add(new()
            {
                Title = "Reassign",
                Href = "/pages/participants/reassign",
            });
        }

        if (await PassesPolicy(principal, SecurityPolicies.Transfers))
        {
            participants.MenuItems.Add(new()
            {
                Title = "Transfers",
                Href = "/pages/participants/transfers",
            });
        }

        participants.MenuItems.Add(new()
        {
            Title = "Active PRI's",
            Href = "/pages/participants/pre-release-inventory",
        });
        return participants;
        
    }

    private async Task<MenuSectionItemModel> CreateDashboardMenu(ClaimsPrincipal principal)
    {
        // dashboards
        MenuSectionItemModel dashboards = new()
        {
            Title = "Dashboards",
            Icon = Icons.Material.Filled.Home,
            IsParent = true,
            PageStatus = PageStatus.Completed,
            MenuItems = []
        };

        // all users have access to the home dashboard
        dashboards.MenuItems.Add(new()
        {
            Title = "Home",
            Href = "/",
        });

        //all users have access to the support worker dashboard
        dashboards.MenuItems.Add(new()
        {
            Title = "Support Worker",
            Href = "/pages/dashboard/supportworker/",
            PageStatus = PageStatus.Wip
        });

        if (await PassesPolicy(principal, SecurityPolicies.ContractData))
        {
            dashboards.MenuItems.Add(new()
            {
                Title = "Contract",
                Href = "/pages/dashboard/contract",
                PageStatus = PageStatus.Wip,
            });
        }

        if (await PassesPolicy(principal, SecurityPolicies.UserHasAdditionalRoles))
        {
            dashboards.MenuItems.Add(new()
            {
                Title = "Tenant",
                Href = "/pages/dashboard/tenant/",
                PageStatus = PageStatus.Wip,
            });
        }

        if (await PassesPolicy(principal, SecurityPolicies.Qa1))
        {
            dashboards.MenuItems.Add(new()
            {
                Title = "Quality Assurance",
                Href = "/pages/dashboard/qualityassurance/",
            });
        }

        if (await PassesPolicy(principal, SecurityPolicies.OutcomeQualityDipChecks))
        {
            dashboards.MenuItems.Add(new()
            {
                Title = "Performance",
                Href = "/pages/dashboard/performance/",
            });
        }

        return dashboards;
    }

    private async Task<bool> PassesPolicy(ClaimsPrincipal principal, string policy)
    {
        var result = await authorizationService.AuthorizeAsync(principal, policy);
        return result.Succeeded;
    }
}