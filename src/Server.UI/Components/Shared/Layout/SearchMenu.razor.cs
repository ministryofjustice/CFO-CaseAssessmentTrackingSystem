using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class SearchMenu
{
    [Inject] private IAsyncMenuService MenuService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    private sealed record PageEntry(string DisplayText, string Href);

    private MudTextField<string>? _searchField;

    private readonly List<PageEntry> _pages = [];
    private List<PageEntry> _filteredPages = [];
    private List<ParticipantSearchResultDto> _participants = [];

    private UserProfile? _userProfile;
    private string _search = string.Empty;
    private bool _searching;
    private bool _loaded;
    private bool _shouldFocus;

    protected override void OnInitialized() => NavigationManager.LocationChanged += OnLocationChanged;

    protected override async Task OnParametersSetAsync()
    {
        if (Open && !_loaded)
        {
            await LoadAsync();
            _shouldFocus = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldFocus && _searchField is not null)
        {
            _shouldFocus = false;
            await _searchField.FocusAsync();
        }
    }

    private async Task LoadAsync()
    {
        var state = await AuthState;
        _userProfile = state.User.GetUserProfileFromClaim();

        var menu = await MenuService.GetFeaturesAsync(state.User);

        _pages.Clear();
        foreach (var section in menu.Sections)
        {
            foreach (var item in section.Links)
            {
                switch (item)
                {
                    case NavigationMenuItemLinkModel { Href: { Length: > 0 } href } link:
                        _pages.Add(new PageEntry(link.DisplayText, href));
                        break;
                    case NavigationMenuItemButtonModel { Href: { Length: > 0 } href } button:
                        _pages.Add(new PageEntry(button.DisplayText, href));
                        break;
                }
            }
        }

        _loaded = true;
    }

    private async Task OnSearchChanged(string value)
    {
        _search = value.Trim();

        if (string.IsNullOrWhiteSpace(_search))
        {
            _filteredPages = [];
            _participants = [];
            return;
        }

        _filteredPages = _pages
            .Where(p => p.DisplayText.Contains(_search, StringComparison.InvariantCultureIgnoreCase))
            .OrderBy(p => p.DisplayText)
            .ToList();

        await SearchParticipantsAsync();
    }

    private async Task SearchParticipantsAsync()
    {
        if (_userProfile is null)
        {
            return;
        }

        _searching = true;

        try
        {
            var result = await GetNewMediator().Send(new SearchParticipants.Query
            {
                Keyword = _search,
                CurrentUser = _userProfile
            }, ComponentCancellationToken);

            _participants = result.Succeeded && result.Data is not null
                ? result.Data
                : [];
        }
        catch (OperationCanceledException)
        {
            // Component disposed or navigation occurred; ignore.
        }
        finally
        {
            _searching = false;
        }
    }

    private void NavigateToParticipant(string participantId)
    {
        _ = CloseAsync();
        NavigationManager.NavigateTo($"/pages/workspace/participants/{participantId}?from=search");
    }

    private async Task CloseAsync()
    {
        Open = false;
        await OpenChanged.InvokeAsync(false);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (Open)
        {
            Open = false;
            _ = OpenChanged.InvokeAsync(false);
            InvokeAsync(StateHasChanged);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose(disposing);
    }
}
