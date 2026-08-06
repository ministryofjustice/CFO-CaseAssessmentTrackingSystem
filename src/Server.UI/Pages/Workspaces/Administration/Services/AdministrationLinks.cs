using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;

public static class AdministrationLinks
{
    public static BreadcrumbLinkModel Home => new("Administration", "", "/pages/workspace/administration");
    public static readonly BreadcrumbLinkModel Jobs = new("Jobs", "View and manage quartz jobs.", $"{Home.Href}/jobs", Group: "System Management");
    public static readonly BreadcrumbLinkModel CacheManagement = new("Cache Management", "Manage the in memory cache for CATS", $"{Home.Href}/cachemanagement", Group: "System Management");
    public static readonly BreadcrumbLinkModel AuditTrails = new("Audit Trails", "View the system audit of edits, updates and inserts", $"{Home.Href}/audittrails", Group: "System Management");
    public static readonly BreadcrumbLinkModel PickList = new("Picklist", "Manage the simple lookup lists.", $"{Home.Href}/picklist", Group: "Meta Data");
    public static readonly BreadcrumbLinkModel Outbox = new("Outbox Messages", "View and reschedule outbox messages.", $"{Home.Href}/outbox", "System Management");
    public static readonly BreadcrumbLinkModel Labels = new("Labels", "Manage labels that can be added to participants", $"{Home.Href}/labels", Group: "Meta Data");
    public static readonly BreadcrumbLinkModel Initiatives = new("Initiatives", "Manage initiatives (innovation funds etc)", $"{Home.Href}/initiatives", Group: "Meta Data");
    public static readonly BreadcrumbLinkModel Tenants = new("Tenants", "Manage Tenants", $"{Home.Href}/tenants", Group: "User Management", 3);
    public static readonly BreadcrumbLinkModel Users = new("Users", "Manage Users", $"{Home.Href}/users/users", Group: "User Management", Order: 1);
    public static readonly BreadcrumbLinkModel UserAudit = new("User Audit", "View user login and authentication audit trails", $"{Home.Href}/users/useraudit", "User Management", Order: 2);
}
 