using System.Drawing;
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
                Title = "QA",
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
                                Roles = [ RoleNames.QAFinance, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title = "QA1",
                                PageStatus = PageStatus.Completed,
                                Href="/pages/qa/enrolments/qa1",
                                Roles = [ RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
                            },
                            new()
                            {
                                Title = "QA2",
                                PageStatus = PageStatus.Completed,
                                Href="/pages/qa/enrolments/qa2",
                                Roles = [  RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport ]
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
                                Title = "Roles",
                                Href = "/identity/roles",
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
                                Title = "Picklist",
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
