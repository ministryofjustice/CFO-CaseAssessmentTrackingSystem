using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Services;

public static class ServiceDeskLinks
{
    public static BreadcrumbLinkModel Home => new("Service Desk", string.Empty, "/pages/workspace/servicedesk");
    public static BreadcrumbLinkModel ActivitiesQueue = new("Activities Queue", "Access relevant Activities queue(s)", $"{Home.Href}/activities/queue", Group: "Queues", Order: 2);
    public static BreadcrumbLinkModel ActivitiesFeedback = new("Activities Feedback", "Access Activities feedback", $"{Home.Href}/activities/feedback", Group: "Reporting", Order: 2);
    public static BreadcrumbLinkModel EnrolmentsQueue = new("Enrolments Queue", "Access relevant Enrolments queue(s)", $"{Home.Href}/enrolments/queue", Group: "Queues", Order: 1);
    public static BreadcrumbLinkModel EnrolmentsFeedback = new("Enrolments Feedback", "Access Enrolments feedback", $"{Home.Href}/enrolments/feedback", Group: "Reporting", Order: 1);
    public static BreadcrumbLinkModel SyncInfo = new("Sync Information", "Access Sync information", $"{Home.Href}/sync/syncinfo", Group: "Reporting", Order: 4);
    public static BreadcrumbLinkModel QaPots = new("QA Pots", "QA pots activity" , $"{Home.Href}/qapots", Group: "Reporting", Order: 3);
}
