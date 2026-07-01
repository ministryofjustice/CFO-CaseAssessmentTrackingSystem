using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Services;

public static class ProviderLinks
{
    public static BreadcrumbLinkModel Home => new ( "Provider", "" ,"/pages/workspace/provider");
    public static BreadcrumbLinkModel CaseWorkload => new ( "Case Workload", "An overview of how cases are spread across your team" , $"{Home.Href}/case-workload");
}
