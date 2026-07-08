using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;

public static class AdministrationLinks
{
    public static BreadcrumbLinkModel Home => new("Administration", "", "/pages/workspace/administration");
    public static readonly BreadcrumbLinkModel Jobs = new("Jobs", "View and manage quarz jobs.", $"{Home.Href}/Jobs");
    public static readonly BreadcrumbLinkModel CacheManagement = new("Cache Management", "Manage the in memory cache for CATS", $"{Home.Href}/CacheManagement");
    public static readonly BreadcrumbLinkModel AuditTrails = new("Audit Trails", "View the system audit of edits, updates and inserts", $"{Home.Href}/AuditTrails");
    public static readonly BreadcrumbLinkModel PickList = new("Picklist", "Manage the simple lookup lists.", $"{Home.Href}/picklist");
    public static readonly BreadcrumbLinkModel Outbox = new("Outbox Messages", "View and reschedule outbox messages.", $"{Home.Href}/Outbox");
    public static readonly BreadcrumbLinkModel Labels = new("Labels", "Manage labels that can be added to participants", $"{Home.Href}/Labels");
    public static readonly BreadcrumbLinkModel Initiatives = new("Initiatives", "Manage initiatives (innovation funds etc)", $"{Home.Href}/Initiatives");
    public static readonly BreadcrumbLinkModel Tenants = new("Tenants", "Manage Tenants", $"{Home.Href}/Tenants");
    public static readonly BreadcrumbLinkModel Users = new("Users", "Manage Users", $"{Home.Href}/users/Users");
    public static readonly BreadcrumbLinkModel UserAudit = new("User Audit", "View user login and authentication audit trails", $"{Home.Href}/users/UserAudit");
}
 