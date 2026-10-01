using Cfo.Cats.Server.UI.Services.Help;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cfo.Cats.Server.UI.Components.Shared.Help;

/// <summary>
/// A drop-in replacement for <see cref="MudTabs"/> that reports its active tab's name to
/// <see cref="HelpContextService"/>, so the help icon knows which tab is currently being viewed.
/// </summary>
/// <remarks>
/// Every existing (and future) <c>&lt;MudTabs&gt;</c> in the app is transparently substituted
/// for this type by <see cref="HelpAwareComponentActivator"/> - no `.razor` file references this
/// class directly. See that class for why this approach was chosen.
///
/// Only the outermost <c>MudTabs</c> on a page reports itself. MudBlazor's own
/// <c>MudTabs.razor</c> markup cascades <c>this</c> (typed as <see cref="MudTabs"/>) down to
/// every descendant, which is how child <c>MudTabPanel</c>s normally find their parent. We
/// reuse that same cascaded value here: if <see cref="AncestorTabs"/> is non-null, this
/// instance is nested inside another MudTabs (e.g. the "Case Summary" tab's inner
/// Case Summary/Status History/Location History tabs) and is deliberately ignored.
/// </remarks>
public class HelpAwareMudTabs : MudTabs
{
    [CascadingParameter]
    private MudTabs? AncestorTabs { get; set; }

    [Inject]
    private HelpContextService HelpContext { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (AncestorTabs is not null)
        {
            // Nested tabs are ignored - only the page-level tabs count.
            return;
        }

        HelpContext.SetTabName(ActivePanel?.Text);
    }
}
