using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Services;

public static class ProviderLinks
{
    public static BreadcrumbLinkModel Home => new ( "Provider", "" ,"/pages/workspace/provider");
    public static BreadcrumbLinkModel CaseWorkload => new ( "Case Workload", "An overview of how cases are spread across your team" , $"{Home.Href}/case-workload");
    public static BreadcrumbLinkModel CaseManagement => new ( "Case Management", "Location, pathway plan and initiative breakdowns for your cases" , $"{Home.Href}/case-management");
    public static BreadcrumbLinkModel Performance => new ( "Performance", "Enrolment, activity and outcome performance for your cases" , $"{Home.Href}/performance");
    public static BreadcrumbLinkModel Cumulatives => new ( "Cumulatives", "Cumulative performance figures against contract targets" , $"{Home.Href}/cumulatives");
    public static BreadcrumbLinkModel EnrolmentsPqa => new ( "Enrolments PQA", "Review enrolments submitted for provider quality assurance" , $"{Home.Href}/enrolments/pqa");
    public static BreadcrumbLinkModel ActivitiesPqa => new ( "Activities PQA", "Review activities submitted for provider quality assurance" , $"{Home.Href}/activities/pqa");
    public static BreadcrumbLinkModel Payments => new ( "Payments", "Contract payment auditing and management" , $"{Home.Href}/payments");
    public static BreadcrumbLinkModel RecentApprovedActivities => new ( "Recent Approved Activities", "View recent approved activities" , $"{Home.Href}/recent-approved-activities");
}
