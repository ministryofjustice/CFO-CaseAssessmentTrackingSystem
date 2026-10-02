using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Cfo.Cats.Server.UI.Services.Help;

/// <summary>
/// Tracks the "help context" (current page key + active top-level tab, if any) for the
/// current circuit, so that the help icon can look up the relevant <see cref="Cfo.Cats.Domain.HelpLinks.HelpLink"/>
/// without any individual page needing to report its own identity.
/// </summary>
/// <remarks>
/// The page key is derived automatically from the current URL and, once available, the
/// router's matched route values (see <see cref="SetRouteData"/>, called from <c>Routes.razor</c>'s
/// <c>&lt;Found&gt;</c> template) via <see cref="HelpPageKeyNormalizer"/>. The active tab name is
/// reported by <see cref="Components.Shared.Help.HelpAwareMudTabs"/>, which transparently replaces
/// every top-level <c>MudTabs</c> instance in the app (see <see cref="HelpAwareComponentActivator"/>)
/// - nested tabs do not report themselves.
/// </remarks>
public class HelpContextService : IDisposable
{
    private readonly NavigationManager _navigationManager;

    public HelpContextService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        PageKey = ResolvePageKey(null);
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    /// <summary>
    /// Raised whenever the current page key or active tab name changes.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// The normalized page key for the page currently being viewed.
    /// </summary>
    public string PageKey { get; private set; }

    /// <summary>
    /// The name of the currently active top-level tab, if the current page has tabs.
    /// Null when the page has no tabs, or no tab has reported itself yet.
    /// </summary>
    public string? TabName { get; private set; }

    /// <summary>
    /// Called by <see cref="Components.Shared.Help.HelpAwareMudTabs"/> whenever the active
    /// top-level tab changes.
    /// </summary>
    public void SetTabName(string? tabName)
    {
        if (TabName == tabName)
        {
            return;
        }

        TabName = tabName;
        Changed?.Invoke();
    }

    /// <summary>
    /// Called by <c>Routes.razor</c>'s <c>&lt;Found&gt;</c> template on every successful route
    /// match, supplying the route values ASP.NET actually matched (e.g. <c>{ "id": "A1234BC" }</c>).
    /// This lets the page key use the real route parameter name/shape rather than guessing from
    /// the value's format, so it works for any id type (guid, int, NOMIS number, CRN, etc.).
    /// </summary>
    public void SetRouteData(Microsoft.AspNetCore.Components.RouteData routeData)
    {
        var newPageKey = ResolvePageKey(routeData.RouteValues);

        // Reset the tab context on navigation - a HelpAwareMudTabs on the new page (if any)
        // will report its own active tab again as soon as it renders.
        var changed = newPageKey != PageKey || TabName is not null;

        PageKey = newPageKey;
        TabName = null;

        if (changed)
        {
            Changed?.Invoke();
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var newPageKey = ResolvePageKey(null);

        // Reset the tab context on navigation - a HelpAwareMudTabs on the new page (if any)
        // will report its own active tab again as soon as it renders. SetRouteData (called
        // from Routes.razor immediately after routing) will refine this page key further once
        // the matched route values are known.
        var changed = newPageKey != PageKey || TabName is not null;

        PageKey = newPageKey;
        TabName = null;

        if (changed)
        {
            Changed?.Invoke();
        }
    }

    private string ResolvePageKey(IReadOnlyDictionary<string, object?>? routeValues)
    {
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        return HelpPageKeyNormalizer.Normalize(uri.AbsolutePath, routeValues);
    }

    public void Dispose() => _navigationManager.LocationChanged -= OnLocationChanged;
}

