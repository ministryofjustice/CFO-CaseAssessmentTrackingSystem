using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;

public static class AdministrationLinks
{
    public static BreadcrumbLinkModel Home => new("Administration", "", "/pages/workspace/Administration");
    public static readonly BreadcrumbLinkModel Jobs = new("Jobs", "Job Scheduler", $"{Home.Href}/Jobs");
    public static readonly BreadcrumbLinkModel CacheManagement = new("Cache Management", "Cache Management", $"{Home.Href}/CacheManagement");
    public static readonly BreadcrumbLinkModel AuditTrails = new("Audit Trails", "Audit Trails", $"{Home.Href}/AuditTrails");
    public static readonly BreadcrumbLinkModel PickList = new("Picklist", "Picklist", $"{Home.Href}/picklist");
    public static readonly BreadcrumbLinkModel Outbox = new("Outbox Messages", "Outbox Messages", $"{Home.Href}/Outbox");
    public static readonly BreadcrumbLinkModel Labels = new("Labels", "Labels", $"{Home.Href}/Labels");
    public static readonly BreadcrumbLinkModel Initiatives = new("Initiatives", "Initiatives", $"{Home.Href}/Initiatives");
    
}
 