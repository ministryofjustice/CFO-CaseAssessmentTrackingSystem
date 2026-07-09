using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Services;

public static class DashboardLinks
{
    public static BreadcrumbLinkModel Home => new("Dashboard", "Home page for dashboard", "/pages/dashboard");
}
