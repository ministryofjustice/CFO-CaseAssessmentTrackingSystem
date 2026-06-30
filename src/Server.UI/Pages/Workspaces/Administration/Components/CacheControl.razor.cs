using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components;

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
