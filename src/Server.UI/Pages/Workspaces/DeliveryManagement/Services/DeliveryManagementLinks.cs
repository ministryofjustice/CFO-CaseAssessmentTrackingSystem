using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Services;

public static class DeliveryManagementLinks
{

    public static BreadcrumbLinkModel Home => new ( "Delivery Management", "" ,"/pages/workspace/deliverymanagement");
   
    // Targets
    public static BreadcrumbLinkModel Performance => new ( "Performance", "Enrolment, activity and outcome performance for your cases" , $"{Home.Href}/performance", Group: TargetsAndDelivery, Order: 1);
    public static BreadcrumbLinkModel Payments => new ( "Payments", "Contract payment auditing and management" , $"{Home.Href}/payments", Group: TargetsAndDelivery, Order: 2);
    public static BreadcrumbLinkModel Cumulatives => new ( "Cumulatives", "Cumulative performance figures against contract targets" , $"{Home.Href}/cumulatives", Group: TargetsAndDelivery, Order: 3);
    public static BreadcrumbLinkModel LocationBreakdown => new ( "Location Breakdown", "View breakdown of cases by location" , $"{Home.Href}/location-breakdown", Group: TargetsAndDelivery, Order: 4);
    public static BreadcrumbLinkModel EngagementsByLocation => new ( "Engagements by Location", "See what activities are taking place in your locations" , $"{Home.Href}/engagements", Group: TargetsAndDelivery, Order: 5);

    /// Case Managment
    public static BreadcrumbLinkModel CaseWorkload => new ( "Case Workload", "An overview of how cases are spread across your team" , $"{Home.Href}/case-workload", Group: CaseManagement, Order: 7);
    public static BreadcrumbLinkModel Initiatives => new ( "Initiatives", "View initiative objectives across your cases" , $"{Home.Href}/initiatives", Group: CaseManagement, Order: 8);
    public static BreadcrumbLinkModel LatestEngagements => new ( "Latest Engagements", "Based on where the participant is currently" , $"{Home.Href}/latest-engagements", Group: CaseManagement, Order: 9);
    public static BreadcrumbLinkModel PathwayPlanReviews => new ( "Pathway Plan Reviews", "Review pathway plan activity for your cases" , $"{Home.Href}/pathway-plan-reviews", Group: CaseManagement, Order: 10);
    public static BreadcrumbLinkModel RiskDue => new ("Risk Due", "View and manage upcoming risk", $"{Home.Href}/riskdue", Group: CaseManagement, Order: 11);

    /// QA Functions
    public static BreadcrumbLinkModel EnrolmentsPqa => new ( "Enrolments PQA", "Review enrolments submitted for provider quality assurance" , $"{Home.Href}/enrolments/pqa", Group: QAFunctions, Order: 12);
    public static BreadcrumbLinkModel ActivitiesPqa => new ( "Activities PQA", "Review activities submitted for provider quality assurance" , $"{Home.Href}/activities/pqa", Group: QAFunctions, Order: 13);
    public static BreadcrumbLinkModel UnassignedCases => new ( "Unassigned Cases", "View unassigned cases by tenant" , $"{Home.Href}/unassigned-cases", Group: QAFunctions, Order: 14);
    public static BreadcrumbLinkModel ArchivedCaseBehaviour => new ("Archived Case Behaviour", "View participants moving into and out of archiving" , $"{Home.Href}/archived-case-behaviour", Group: QAFunctions, Order: 15);
    public static BreadcrumbLinkModel ActivitiesInQaPots => new ( "Activities in QA Pots", "View your activities awaiting quality assurance" , $"{Home.Href}/activities-in-qa-pots", Group: QAFunctions, Order: 16);
    public static BreadcrumbLinkModel QaEnrolmentResults => new ( "QA Enrolment Results", "View enrolment activity", $"{Home.Href}/qa-enrolment-results", Group: QAFunctions, Order: 17);
    public static BreadcrumbLinkModel RecentApprovedActivities => new ( "Recent Approved Activities", "View recent approved activities" , $"{Home.Href}/recent-approved-activities", Group: QAFunctions, Order: 18);
    
    private static string TargetsAndDelivery = "Targets and Delivery";
    private static string CaseManagement = "Case Management";
    private static string QAFunctions = "QA Functions";

}
