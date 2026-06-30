using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;

public static class AdministrationLinks
{
    public static BreadcrumbLinkModel Home => new("Administration", "", "/pages/workspace/Administration");
    public static BreadcrumbLinkModel Jobs = new("Jobs", "Job Scheduler", $"{Home.Href}/Jobs");
    public static BreadcrumbLinkModel CacheManagement = new("Cache Management", "Cache Management", $"{Home.Href}/CacheManagement");
    public static BreadcrumbLinkModel SyncInformation = new("Sync Information", "Sync Information", $"{Home.Href}/SyncInformation");
 }
 