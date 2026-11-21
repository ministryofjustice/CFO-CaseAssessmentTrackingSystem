using System.Reflection;
using System.Security.Claims;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public sealed record WorkspaceLink(Workspace Workspace, BreadcrumbLinkModel Link);

/// <summary>
/// Returns the workspace sub pages a user may see. Used by the workspace landing pages and
/// by global search so both apply the same rules.
/// </summary>
public interface IWorkspaceLinkProvider
{
    Task<IReadOnlyList<BreadcrumbLinkModel>> GetLinksAsync(Workspace workspace, ClaimsPrincipal user);

    Task<IReadOnlyList<WorkspaceLink>> GetAllLinksAsync(ClaimsPrincipal user);

    /// <summary>
    /// Of the supplied paths, returns those that are registered pages the user is not allowed to see.
    /// </summary>
    Task<IReadOnlySet<string>> GetRestrictedPathsAsync(IEnumerable<string> paths, ClaimsPrincipal user);
}

public sealed class WorkspaceLinkProvider(IAuthorizationService authorizationService, IAuthorizationPolicyProvider policyProvider, IConfiguration configuration)
    : IWorkspaceLinkProvider
{
    public async Task<IReadOnlyList<BreadcrumbLinkModel>> GetLinksAsync(Workspace workspace, ClaimsPrincipal user)
        => (await EvaluateAsync(user, workspace)).Select(r => r.Link).ToArray();

    public async Task<IReadOnlyList<WorkspaceLink>> GetAllLinksAsync(ClaimsPrincipal user)
        => (await EvaluateAsync(user, null)).Select(r => new WorkspaceLink(r.Workspace, r.Link)).ToArray();

    public async Task<IReadOnlySet<string>> GetRestrictedPathsAsync(IEnumerable<string> paths, ClaimsPrincipal user)
    {
        var evaluator = new WorkspaceLinkEvaluator(user, authorizationService, configuration);
        var restricted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in paths.Select(p => p.TrimEnd('/')).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var entry = WorkspaceRegistry.Entries.FirstOrDefault(e => e.Link.Href.Equals(path, StringComparison.OrdinalIgnoreCase));
            if (entry is not null && !await entry.IsVisible(evaluator))
            {
                restricted.Add(path);
            }
            else if (!await CanOpenPageAsync(path, user))
            {
                restricted.Add(path);
            }
        }

        return restricted;
    }

    // Pages not in the registry (workspace roots, queue processing pages) are checked the way the framework
    // does it: combine the page's [Authorize] metadata (including the default policy) and authorize against it
    private async Task<bool> CanOpenPageAsync(string path, ClaimsPrincipal user)
    {
        var page = PageRoutes.Resolve(path);
        if (page is null || page.GetCustomAttributes<AllowAnonymousAttribute>().Any())
        {
            return true;
        }

        var policy = await AuthorizationPolicy.CombineAsync(policyProvider, page.GetCustomAttributes<AuthorizeAttribute>());

        return policy is null || (await authorizationService.AuthorizeAsync(user, policy)).Succeeded;
    }

    private async Task<List<WorkspaceEntry>> EvaluateAsync(ClaimsPrincipal user, Workspace? workspace)
    {
        var evaluator = new WorkspaceLinkEvaluator(user, authorizationService, configuration);
        var visible = new List<WorkspaceEntry>();

        foreach (var entry in WorkspaceRegistry.Entries.Where(e => workspace is null || e.Workspace == workspace))
        {
            if (await entry.IsVisible(evaluator))
            {
                visible.Add(entry);
            }
        }

        return visible;
    }
}
