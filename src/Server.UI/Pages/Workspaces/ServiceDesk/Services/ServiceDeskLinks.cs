using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Cfo.Cats.Domain.Labels.Rules;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.StaticAssets;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Services;

public static class ServiceDeskLinks
{
    public static BreadcrumbLinkModel Home => new("Service Desk", string.Empty, "/pages/workspace/servicedesk");
    public static BreadcrumbLinkModel ActivitiesQueue = new("Activities Queue", "Activities Queue", $"{Home.Href}/activities/queue");
    public static BreadcrumbLinkModel ActivitiesFeedback = new("Activities Feedback", "Activities Feedback", $"{Home.Href}/activities/feedback");
    public static BreadcrumbLinkModel EnrolmentsQueue = new("Enrolments Queue", "Enrolments Queue", $"{Home.Href}/enrolments/queue");
    public static BreadcrumbLinkModel EnrolmentsFeedback = new("Enrolments Feedback", "Enrolments Feedback", $"{Home.Href}/enrolments/feedback");

}

