using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components;

[Authorize(Policy = SecurityPolicies.SystemSupportFunctions)]
public partial class CacheControl
{
    [Inject] 
    private IFusionCache FusionCache { get; set; } = null!;

    private async Task ClearCache()
    {
        await FusionCache.ClearAsync();
        Snackbar.Add($"Cache cleared", Severity.Info);
    }
}
