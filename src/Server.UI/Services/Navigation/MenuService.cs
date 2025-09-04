using ActualLab.Api;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using System.Linq.Expressions;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public class MenuService : IMenuService
{
    public IEnumerable<MenuSectionModel> Features =>
        [
            new()
            {
                Title = "Application",
                SectionItems =
                [
                    new()
                    {
                        Title = "Dashboards",
                        Icon = Icons.Material.Filled.Home,
                        IsParent = true,
                        PageStatus = PageStatus.Completed,
                        MenuItems =
                        [
                            new()
                            {
                                Title = "Home",
                                Href = "/",
                                PageStatus = PageStatus.Completed
                            },

                            new()
                            {
                                Title = "Support Worker Performance",
                                Href = "/pages/dashboard/supportworker/performance",
                                PageStatus = PageStatus.Wip
                            }
                        ],
                    },

                    new()
                    {
                        Title = "Participants",
                        Icon = Icons.Material.Filled.EmojiPeople,
                        PageStatus = PageStatus.Completed,
                        IsParent = true,
                        MenuItems =
                        [
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
                                Roles = [RoleNames.QAFinance, RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            },
                            new()
                            {
                                Title = "Transfers",
                                Href = "/pages/participants/transfers",
                                PageStatus = PageStatus.Completed,
                                Roles = [RoleNames.QAFinance, RoleNames.QAOfficer, RoleNames.PerformanceManager, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            },
                            new()
                            {
                                Title = "Active PRI's",
                                Href = "/pages/participants/pre-release-inventory",
                                PageStatus = PageStatus.Completed
                            }
                        ]
                    },
                    new()
                    {
                        Title = "Analytics",
                        Icon = Icons.Material.Filled.Analytics,
                        PageStatus = PageStatus.Completed,
                        IsParent = true,
                        MenuItems =
                        [
                            new()
                            {
                                Title = "Cumulatives",
                                Href = "/pages/analytics/cumulatives",
                                PageStatus = PageStatus.Completed,
                                Roles =
                                [
                                    RoleNames.SystemSupport, RoleNames.Finance
                                ],
                            },
                            new()
                            {
                                Title = "My Documents",
                                Href = "/pages/analytics/my-documents",
                                PageStatus = PageStatus.Completed
                            },
                            new()
                            {
                                Title = "Outcome Quality",
                                Href = "/pages/analytics/outcome-quality-dip-sampling",
                                PageStatus = PageStatus.Completed,
                                Roles = [RoleNames.SystemSupport, RoleNames.SMT, RoleNames.PerformanceManager, RoleNames.CSO, RoleNames.CPM, RoleNames.CMPSM],
                            }
                        ]
                    }

                ]
            },
            new()
            {
                Title = "Quality Control",
                Roles = [RoleNames.SystemSupport, RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.QAFinance],
                SectionItems =
                [
                    new()
                    {
                        IsParent = true,
                        Title = "Enrolments",
                        Icon = Icons.Material.Filled.Approval,
                        MenuItems =
                        [
                            new()
                            {
                                Title = "PQA",
                                Href = "/pages/qa/enrolments/pqa",
                                PageStatus = PageStatus.Completed,
                                Roles = [RoleNames.QAFinance, RoleNames.SMT, RoleNames.SystemSupport]
                            },

                            new()
                            {
                                Title = "Queue Management",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/servicedesk/enrolments",
                                Roles = [RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            },

                            new()
                            {
                                Title = "First Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/enrolments/qa1/",
                                Roles = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            },

                            new()
                            {
                                Title = "Second Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/enrolments/qa2/",
                                Roles = [RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            }
                        ]
                    },

                    new()
                    {
                        IsParent = true,
                        Title = "Activities",
                        Icon = Icons.Material.Filled.SelfImprovement,
                        MenuItems =
                        [
                            new()
                            {
                                Title = "PQA",
                                Href = "/pages/qa/activities/pqa",
                                PageStatus = PageStatus.Completed,
                                Roles = [RoleNames.QAFinance, RoleNames.SMT, RoleNames.SystemSupport]
                            },

                            new()
                            {
                                Title = "Queue Management",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/activities/activities",
                                Roles = [RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            },

                            new()
                            {
                                Title = "First Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/activities/qa1/",
                                Roles = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            },

                            new()
                            {
                                Title = "Second Pass",
                                PageStatus = PageStatus.Completed,
                                Href = "/pages/qa/activities/qa2/",
                                Roles = [RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport]
                            }
                        ]
                    }
                ]
            },
            new()
            {
                Title = "Finance",
                
                Roles = [RoleNames.SystemSupport, RoleNames.Finance],
                SectionItems = new ApiList<MenuSectionItemModel>
                {
                    new()
                    {
                        IsParent = false,
                        Title = "Payments",
                        Icon = Icons.Material.Filled.Money,
                        Href = "/pages/finance/payments/"
                    }
                }
            },
            new()
            {
                Title = "MANAGEMENT",
                Roles = [RoleNames.SystemSupport, RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT],
                SectionItems =
                [
                    new()
                    {
                        IsParent = true,
                        Title = "Authorization",
                        Icon = Icons.Material.Filled.ManageAccounts,
                        MenuItems =
                        [
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
                                Title = "User Audit",
                                Href = "/identity/users/audit",
                                PageStatus = PageStatus.Completed
                            },

                            new()
                            {
                                Title = "Profile",
                                Href = "/user/profile",
                                PageStatus = PageStatus.Completed
                            }
                        ]
                    },

                    new()
                    {
                        IsParent = true,
                        Title = "System",
                        Icon = Icons.Material.Filled.Devices,
                        MenuItems =
                        [
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
                            },

                            new()
                            {
                                Title = "Outbox Messages",
                                Href = "/system/outbox",
                                PageStatus = PageStatus.Completed
                            }
                        ]
                    }
                ]
            }
        ];
}
