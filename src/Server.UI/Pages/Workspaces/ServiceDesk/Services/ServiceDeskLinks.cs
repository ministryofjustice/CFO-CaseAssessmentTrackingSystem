using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Services;

public static class ServiceDeskLinks
{
    public static BreadcrumbLinkModel Home => new("Service Desk", string.Empty, "/pages/workspace/servicedesk");
    public static BreadcrumbLinkModel ActivitiesQueue = new("Activities Queue", "Access relevant Activities queue(s)", $"{Home.Href}/activities/queue");
    public static BreadcrumbLinkModel ActivitiesFeedback = new("Activities Feedback", "Access Activities feedback", $"{Home.Href}/activities/feedback");
    public static BreadcrumbLinkModel EnrolmentsQueue = new("Enrolments Queue", "Access relevant Enrolments queue(s)", $"{Home.Href}/enrolments/queue");
    public static BreadcrumbLinkModel EnrolmentsFeedback = new("Enrolments Feedback", "Access Enrolments feedback", $"{Home.Href}/enrolments/feedback");
    public static BreadcrumbLinkModel SyncInfo = new("Sync Information", "Access Sync information", $"{Home.Href}/sync/syncinfo");
    public static BreadcrumbLinkModel QaPots = new("QA Pots", "QA pots activity" , $"{Home.Href}/qapots");
}
