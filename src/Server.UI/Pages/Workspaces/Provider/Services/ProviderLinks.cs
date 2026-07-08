using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Services;

public static class ProviderLinks
{
    public static BreadcrumbLinkModel Home => new ( "Provider", "" ,"/pages/workspace/provider");
    public static BreadcrumbLinkModel CaseWorkload => new ( "Case Workload", "An overview of how cases are spread across your team" , $"{Home.Href}/case-workload");
    public static BreadcrumbLinkModel CaseManagement => new ( "Case Management", "Location, pathway plan and initiative breakdowns for your cases" , $"{Home.Href}/case-management");
    public static BreadcrumbLinkModel Performance => new ( "Performance", "Enrolment, activity and outcome performance for your cases" , $"{Home.Href}/performance");
    public static BreadcrumbLinkModel Cumulatives => new ( "Cumulatives", "Cumulative performance figures against contract targets" , $"{Home.Href}/cumulatives");
}
