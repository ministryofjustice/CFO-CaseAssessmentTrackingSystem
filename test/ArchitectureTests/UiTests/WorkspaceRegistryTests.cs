using System.Reflection;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Domain.ArchitectureTests.UiTests;

public class WorkspaceRegistryTests
{
    private static readonly Assembly UiAssembly = typeof(WorkspaceRegistry).Assembly;

    private sealed record Page(Type Type, string Route);

    private static readonly List<Page> AllRoutablePages = UiAssembly.GetTypes()
        .Where(t => typeof(IComponent).IsAssignableFrom(t))
        .SelectMany(t => t.GetCustomAttributes<RouteAttribute>().Select(r => new Page(t, Normalise(r.Template))))
        .ToList();

    private static readonly List<Page> WorkspacePages = AllRoutablePages
        .Where(p => p.Type.Namespace?.Contains(".Pages.Workspaces.") == true)
        .ToList();

    private static string Normalise(string route) => "/" + route.Trim('/').ToLowerInvariant();

    [Test]
    public void Registered_links_should_point_at_a_routable_page()
    {
        var missing = WorkspaceRegistry.Entries
            .Where(e => AllRoutablePages.All(p => p.Route != Normalise(e.Link.Href)))
            .Select(e => $"{e.Workspace}: {e.Link.Title} ({e.Link.Href})")
            .ToList();

        missing.ShouldBeEmpty($"Registry links without a page:\n{string.Join("\n", missing)}");
    }

    [Test]
    public void Page_authorization_should_match_the_registry()
    {
        var mismatches = new List<string>();

        foreach (var entry in WorkspaceRegistry.Entries)
        {
            foreach (var page in AllRoutablePages.Where(p => p.Route == Normalise(entry.Link.Href)))
            {
                var authorize = page.Type.GetCustomAttributes<AuthorizeAttribute>().ToList();

                if (entry.Policy is null && entry.Roles is null)
                {
                    var isOpenToAllAuthorizedUsers = authorize.Count > 0
                        && authorize.All(a => a.Roles is null && (a.Policy is null or SecurityPolicies.AuthorizedUser));

                    if (!isOpenToAllAuthorizedUsers)
                    {
                        var actual = authorize.Count == 0
                            ? "no [Authorize]"
                            : string.Join(", ", authorize.Select(a => a.Policy ?? (a.Roles is not null ? $"Roles={a.Roles}" : "[Authorize]")));
                        mismatches.Add($"{page.Type.Name} ({page.Route}) is Open in registry but has {actual}");
                    }

                    continue;
                }

                var policyMatches = entry.Policy is null || authorize.Any(a => a.Policy == entry.Policy);
                var rolesMatch = entry.Roles is null || authorize.Any(a => a.Roles == entry.Roles);

                if (!policyMatches || !rolesMatch)
                {
                    var actual = authorize.Count == 0
                        ? "no [Authorize]"
                        : string.Join(", ", authorize.Select(a => a.Policy ?? $"Roles={a.Roles}"));
                    mismatches.Add($"{page.Type.Name} ({page.Route}) expected {entry.Policy ?? entry.Roles} but has {actual}");
                }
            }
        }

        mismatches.ShouldBeEmpty($"Page policy differs from the registry:\n{string.Join("\n", mismatches)}");
    }

    // QA processing pages are opened from the queues rather than from a workspace menu.
    private static readonly HashSet<string> QueueProcessingPages = ["QA1", "QA2", "QAServiceDesk"];

    [Test]
    public void Workspace_pages_should_be_registered()
    {
        var registered = WorkspaceRegistry.Entries.Select(e => Normalise(e.Link.Href)).ToHashSet();
        var homes = Enum.GetValues<Workspace>().Select(w => Normalise(WorkspaceRegistry.HomeOf(w).Href)).ToHashSet();

        // Pages with route parameters are reached from a parent page and build their own breadcrumbs.
        var unregistered = WorkspacePages
            .Where(p => !p.Route.Contains('{'))
            .Where(p => !QueueProcessingPages.Contains(p.Type.Name))
            .Where(p => !registered.Contains(p.Route) && !homes.Contains(p.Route))
            .Select(p => $"{p.Type.Name} ({p.Route})")
            .OrderBy(x => x)
            .ToList();

        unregistered.ShouldBeEmpty($"Routable workspace pages not in WorkspaceRegistry:\n{string.Join("\n", unregistered)}");
    }
}
