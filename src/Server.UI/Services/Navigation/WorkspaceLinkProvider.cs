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
}

public sealed class WorkspaceLinkProvider(IAuthorizationService authorizationService, IConfiguration configuration)
    : IWorkspaceLinkProvider
{
    public async Task<IReadOnlyList<BreadcrumbLinkModel>> GetLinksAsync(Workspace workspace, ClaimsPrincipal user)
        => (await EvaluateAsync(user, workspace)).Select(r => r.Link).ToArray();

    public async Task<IReadOnlyList<WorkspaceLink>> GetAllLinksAsync(ClaimsPrincipal user)
        => (await EvaluateAsync(user, null)).Select(r => new WorkspaceLink(r.Workspace, r.Link)).ToArray();

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
