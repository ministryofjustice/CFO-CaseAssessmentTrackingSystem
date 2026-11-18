using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Pages;

public partial class Search
{
    [Parameter] public string? Keyword { get; set; }

    [SupplyParameterFromQuery(Name = "q")] public string? QueryKeyword { get; set; }

    private readonly List<PageResult> _pages = [];

    private static readonly Dictionary<Workspace, string> WorkspaceNames = new()
    {
        [Workspace.Participants] = "Participants",
        [Workspace.DeliveryManagement] = "Delivery Management",
        [Workspace.ServiceDesk] = "Service Desk",
        [Workspace.Performance] = "Performance Management",
        [Workspace.Administration] = "Administration",
        [Workspace.MyArea] = "My Area"
    };
    private List<SearchResult> _results = [];
    private string _search = string.Empty;
    private string _searchedKeyword = string.Empty;
    private bool _loading;
    private bool _disposed;
    private int _requestId;
    private string? _participantError;
    private bool _truncated;
    private readonly SemaphoreSlim _searchLock = new(1, 1);
    private const int ParticipantLimit = 25;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        var menu = await MenuService.GetFeaturesAsync(state.User);

        foreach (var section in menu.Sections)
        {
            foreach (var item in section.Links)
            {
                switch (item)
                {
                    case NavigationMenuItemLinkModel { Href: { Length: > 0 } href } link:
                        _pages.Add(new PageResult(link.DisplayText, href, section.Title, link.AccessibilityText, link.Target));
                        break;
                    case NavigationMenuItemButtonModel { Href: { Length: > 0 } href } button:
                        _pages.Add(new PageResult(button.DisplayText, href, section.Title, button.AccessibilityText));
                        break;
                }
            }
        }

        foreach (var (workspace, link) in await LinkProvider.GetAllLinksAsync(state.User))
        {
            _pages.Add(new PageResult(link.Title, link.Href, WorkspaceNames[workspace], link.Description));
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        var effectiveKeyword = !string.IsNullOrWhiteSpace(QueryKeyword)
            ? QueryKeyword
            : Keyword ?? string.Empty;

        _search = effectiveKeyword;
        await SearchAsync(effectiveKeyword.Trim());
    }

    private async Task SearchAsync(string keyword)
    {
        if (_disposed)
        {
            return;
        }

        var requestId = ++_requestId;

        // The mediator scope shares one DbContext, so only one participant lookup may run at a time
        await _searchLock.WaitAsync();
        try
        {
            if (_disposed || requestId != _requestId)
            {
                return;
            }

            _searchedKeyword = keyword;
            _loading = true;
            _participantError = null;
            _truncated = false;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                _results = [];
                return;
            }

            var matchingPages = _pages
                .Where(page => page.DisplayText.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                               || page.Description.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                .Cast<SearchResult>();

            var result = await Service.Send(new SearchParticipants.Query
            {
                Keyword = keyword,
                MaxResults = ParticipantLimit + 1
            });

            if (_disposed || requestId != _requestId)
            {
                return;
            }

            IEnumerable<ParticipantResult> participants = [];
            if (result is { Succeeded: true, Data: not null })
            {
                _truncated = result.Data.Count > ParticipantLimit;
                participants = result.Data.Take(ParticipantLimit).Select(participant => new ParticipantResult(participant));
            }
            else
            {
                _participantError = string.IsNullOrWhiteSpace(result.ErrorMessage) || result.ErrorMessage == "Unexpected error"
                    ? "Participant search failed, so participants are not shown. Please try again."
                    : $"Participants could not be searched: {result.ErrorMessage}";
            }

            _results = matchingPages
                .Concat(participants)
                .OrderBy(searchResult => searchResult.DisplayText)
                .ToList();
        }
        finally
        {
            if (requestId == _requestId)
            {
                _loading = false;
            }

            if (!_disposed)
            {
                _searchLock.Release();
            }
        }
    }

    private Task SearchChangedAsync(string? value)
    {
        _search = value ?? string.Empty;
        return Task.CompletedTask;
    }

    private async Task ClearSearchAsync(MouseEventArgs _)
    {
        _search = string.Empty;
        if (!string.IsNullOrEmpty(QueryKeyword) || !string.IsNullOrEmpty(Keyword))
        {
            Navigation.NavigateTo("/pages/search");
        }
        else
        {
            await SearchAsync(string.Empty);
        }
    }

    private async Task OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key != "Enter")
        {
            return;
        }

        var trimmed = _search.Trim();
        var target = string.IsNullOrWhiteSpace(trimmed)
            ? "/pages/search"
            : $"/pages/search?q={Uri.EscapeDataString(trimmed)}";

        var currentRelative = "/" + Navigation.ToBaseRelativePath(Navigation.Uri);
        if (string.Equals(currentRelative, target, StringComparison.Ordinal))
        {
            await SearchAsync(trimmed);
        }
        else
        {
            Navigation.NavigateTo(target);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            _searchLock.Dispose();
        }

        base.Dispose(disposing);
    }

    private abstract record SearchResult(string DisplayText);

    private sealed record PageResult(string DisplayText, string Href, string Section, string Description, string? Target = null) : SearchResult(DisplayText);

    private sealed record ParticipantResult(ParticipantSearchResultDto Participant)
        : SearchResult(Participant.FullName);
}
