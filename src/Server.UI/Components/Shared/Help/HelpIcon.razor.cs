using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.Features.HelpLinks.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Services.Help;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Components.Shared.Help;

/// <summary>
/// The help icon shown alongside the page breadcrumbs (see <see cref="Layout.PageHeader"/>).
/// Visible to every user when a <see cref="HelpLinkDto"/> is configured for the current
/// page/tab (see <see cref="HelpContextService"/>), and always visible to System Support users
/// (with a warning colour when no link is configured) so they can identify gaps in help
/// coverage as they navigate the app.
/// </summary>
public partial class HelpIcon : IDisposable
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = null!;

    [Inject]
    private HelpContextService HelpContext { get; set; } = null!;

    private HelpLinkDto? _helpLink;
    private bool _isSystemSupport;
    private bool _hasLink => _helpLink is not null;
    private bool _visible => _isSystemSupport || _hasLink;

    // HelpContextService.Changed can fire more than once for a single navigation (location
    // change, matched route data, then the new page's tab reporting itself), and each firing
    // is dispatched independently via InvokeAsync. Without serialising the resulting reloads,
    // two overlapping calls can end up awaiting the same scoped DbContext at once, which throws
    // "A second operation was started on this context instance...". _loadGate ensures only one
    // reload is ever in flight, and _loadVersion lets an in-flight, now-stale reload discard its
    // result instead of overwriting a newer one that completed first.
    private readonly SemaphoreSlim _loadGate = new(1, 1);
    private int _loadVersion;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _isSystemSupport = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemSupportFunctions)).Succeeded;
        HelpContext.Changed += OnHelpContextChanged;
        await LoadHelpLinkAsync();
    }

    private void OnHelpContextChanged() => InvokeAsync(() => LoadHelpLinkAsync());

    private async Task LoadHelpLinkAsync()
    {
        var version = Interlocked.Increment(ref _loadVersion);

        await _loadGate.WaitAsync();
        try
        {
            // A newer reload was queued while we were waiting for the gate - let that one win.
            if (version != _loadVersion)
            {
                return;
            }

            var result = await Service.Send(new GetHelpLinkForPage.Query(HelpContext.PageKey, HelpContext.TabName));

            if (version != _loadVersion)
            {
                return;
            }

            _helpLink = result.Succeeded ? result.Data : null;
            StateHasChanged();
        }
        finally
        {
            _loadGate.Release();
        }
    }

    private async Task OnClickAsync()
    {
        var parameters = new DialogParameters<HelpDialog>
        {
            { x => x.HelpLink, _helpLink },
            { x => x.PageKey, HelpContext.PageKey },
            { x => x.TabName, HelpContext.TabName },
            { x => x.IsSystemSupport, _isSystemSupport }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        await DialogService.ShowAsync<HelpDialog>("Help", parameters, options);
    }

    public void Dispose() => HelpContext.Changed -= OnHelpContextChanged;
}
