using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Services;

public static class DeliveryManagementLinks
{
    public static BreadcrumbLinkModel Home => new ( "Delivery Management", "" ,"/pages/workspace/deliverymanagement");
    public static BreadcrumbLinkModel CaseWorkload => new ( "Case Workload", "An overview of how cases are spread across your team" , $"{Home.Href}/case-workload", Group: Insights);
    public static BreadcrumbLinkModel LocationBreakdown => new ( "Location Breakdown", "View breakdown of cases by location" , $"{Home.Href}/location-breakdown", Group: Insights);
    public static BreadcrumbLinkModel PathwayPlanReviews => new ( "Pathway Plan Reviews", "Review pathway plan activity for your cases" , $"{Home.Href}/pathway-plan-reviews", Group: Insights);
    public static BreadcrumbLinkModel Initiatives => new ( "Initiatives", "View initiative objectives across your cases" , $"{Home.Href}/initiatives", Group: Insights);
    public static BreadcrumbLinkModel UnassignedCases => new ( "Unassigned Cases", "View unassigned cases by tenant" , $"{Home.Href}/unassigned-cases", Group: QA);
    public static BreadcrumbLinkModel Performance => new ( "Performance", "Enrolment, activity and outcome performance for your cases" , $"{Home.Href}/performance", Group: Insights);
    public static BreadcrumbLinkModel Cumulatives => new ( "Cumulatives", "Cumulative performance figures against contract targets" , $"{Home.Href}/cumulatives", Group: Insights);
    public static BreadcrumbLinkModel EnrolmentsPqa => new ( "Enrolments PQA", "Review enrolments submitted for provider quality assurance" , $"{Home.Href}/enrolments/pqa", Group: QA);
    public static BreadcrumbLinkModel ActivitiesPqa => new ( "Activities PQA", "Review activities submitted for provider quality assurance" , $"{Home.Href}/activities/pqa", Group: QA);
    public static BreadcrumbLinkModel Payments => new ( "Payments", "Contract payment auditing and management" , $"{Home.Href}/payments", Group: Insights);
    public static BreadcrumbLinkModel RecentApprovedActivities => new ( "Recent Approved Activities", "View recent approved activities" , $"{Home.Href}/recent-approved-activities", Group: Insights);
    public static BreadcrumbLinkModel ActivitiesInQaPots => new ( "Activities in QA Pots", "View your activities awaiting quality assurance" , $"{Home.Href}/activities-in-qa-pots", Group: QA);

    private static string Insights = "Insights";
    private static string QA = "QA Functions";

}
