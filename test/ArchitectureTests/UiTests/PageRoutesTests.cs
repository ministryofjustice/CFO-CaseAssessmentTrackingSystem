using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using Shouldly;
using System.Reflection;

namespace Cfo.Cats.Domain.ArchitectureTests.UiTests;

public class PageRoutesTests
{
    [TestCase("/pages/workspace/servicedesk/activities/qa2", SecurityPolicies.Qa2)]
    [TestCase("/pages/workspace/servicedesk/activities/qa1/", SecurityPolicies.Qa1)]
    [TestCase("/PAGES/WORKSPACE/SERVICEDESK/ACTIVITIES/QUEUE", SecurityPolicies.ServiceDeskManagement)]
    public void Route_should_resolve_to_the_page_that_requires_the_policy(string path, string expectedPolicy)
    {
        var page = PageRoutes.Resolve(path);

        page.ShouldNotBeNull();
        page!.GetCustomAttributes<AuthorizeAttribute>().Select(a => a.Policy).ShouldContain(expectedPolicy);
    }

    [Test]
    public void Parameterised_route_should_resolve() =>
        PageRoutes.Resolve("/pages/workspace/participants/P000000001").ShouldNotBeNull();

    [Test]
    public void Unknown_route_should_not_resolve() =>
        PageRoutes.Resolve("/pages/not/a/real/page").ShouldBeNull();

    [Test]
    public void Every_workspace_home_should_resolve_to_a_page()
    {
        foreach (var workspace in Enum.GetValues<Workspace>())
        {
            PageRoutes.Resolve(WorkspaceRegistry.HomeOf(workspace).Href).ShouldNotBeNull(workspace.ToString());
        }
    }
}
