using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.NavigationMenu;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public class MenuService : IMenuService
{
    public IEnumerable<MenuSectionModel> Features =>
        [
            new MenuSectionModel
            {
                Title = "Application",
                SectionItems = new List<MenuSectionItemModel>
                {
                    new()
                    {
                        Title = "Dashboard",
                        Icon = Icons.Material.Filled.Home,
                        Href = "/"
                    },
                    new()
                    {
                        Title = "Participants",
                        Icon = Icons.Material.Filled.EmojiPeople,
                        PageStatus = PageStatus.Completed,
                        IsParent = true,
                        MenuItems = new List<MenuSectionSubItemModel>
                        {
                            new()
                            {
                                Title = "All",
                                Href = "/pages/participants",
                                PageStatus = PageStatus.Completed
                            },
                                new()
                            {
                                Title = "Moved Participants",
                                Href = "/pages/participants/movedparticipants",
                                PageStatus = PageStatus.Completed                                
                            },
                            new()
                            {
                                Title = "Reassign",
                                Href = "/pages/participants/reassign",                                
                                PageStatus = PageStatus.Completed,
                                Roles = [RoleNames.QAFinance, RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title = "Transfers",
                                Href = "/pages/participants/transfers",
                                PageStatus = PageStatus.Completed,
                                Roles = [RoleNames.QAFinance, RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                        }
                    },
                    new()
                    {
                        Title = "Reports",
                        Icon = Icons.Material.Filled.Analytics,
                        Href = "/reports",
                        PageStatus = PageStatus.ComingSoon
                    },                  
                }
            },
            new MenuSectionModel
            {
                Title = "Quality Control",
                Roles = [RoleNames.SystemSupport, RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.QAFinance],
                SectionItems = new List<MenuSectionItemModel>
                {
                    new()
                    {
                        IsParent = true,
                        Title = "Enrolments",
                        Icon = Icons.Material.Filled.Approval,
                        MenuItems = new List<MenuSectionSubItemModel>
                        {
                            new()
                            {
                                Title = "PQA",
                                Href="/pages/qa/enrolments/pqa",
                                PageStatus = PageStatus.Completed,
                                Roles = [ RoleNames.QAFinance, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title = "Queue Management",
                                PageStatus = PageStatus.Completed,
                                Href="/pages/qa/servicedesk/enrolments",
                                Roles = [ RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title="First Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/enrolments/qa1/",
                                Roles = [ RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title="Second Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/enrolments/qa2/",
                                Roles = [ RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            }
                        }
                    },
                     new()
                    {
                        IsParent = true,
                        Title = "Activities",
                        Icon = Icons.Material.Filled.SelfImprovement,
                        MenuItems = new List<MenuSectionSubItemModel>
                        {
                            new()
                            {
                                Title = "PQA",
                                Href="/pages/qa/activities/pqa",
                                PageStatus = PageStatus.Completed,
                                Roles = [ RoleNames.QAFinance, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title = "Queue Management",
                                PageStatus = PageStatus.Completed,
                                Href="/pages/qa/activities/activities",
                                Roles = [ RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title="First Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/activities/qa1/",
                                Roles = [ RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title="Second Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/activities/qa2/",
                                Roles = [ RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            }
                        }
                    }
                }
            },
            new MenuSectionModel
            {
                Title = "MANAGEMENT",
                Roles = new[] { RoleNames.SystemSupport, RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT },
                SectionItems = new List<MenuSectionItemModel>
                {
                    new()
                    {
                        IsParent = true,
                        Title = "Authorization",
                        Icon = Icons.Material.Filled.ManageAccounts,
                        MenuItems = new List<MenuSectionSubItemModel>
                        {
                            new()
                            {
                                Title = "Tenants",
                                Href = "/administration/tenants",
                                PageStatus = PageStatus.Completed
                            },
                            new()
                            {
                                Title = "Users",
                                Href = "/identity/users",
                                PageStatus = PageStatus.Completed
                            },
                            new()
                            {
                                Title = "Profile",
                                Href = "/user/profile",
                                PageStatus = PageStatus.Completed
                            }
                        }
                    },
                    new()
                    {
                        IsParent = true,
                        Title = "System",
                        Icon = Icons.Material.Filled.Devices,
                        MenuItems = new List<MenuSectionSubItemModel>
                        {
                            new()
                            {
                                Title = "Lookup Values",
                                Href = "/system/picklist",
                                PageStatus = PageStatus.Completed
                            },
                            new()
                            {
                                Title = "Audit Trails",
                                Href = "/system/audittrails",
                                PageStatus = PageStatus.Completed
                            }
                        }
                    }
                }
            }
        ];
}
