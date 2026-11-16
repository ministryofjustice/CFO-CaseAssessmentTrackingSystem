using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Pages;

public partial class Search
{
    [Parameter] public string Keyword { get; set; } = string.Empty;

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
                        _pages.Add(new PageResult(link.DisplayText, href, section.Title, link.AccessibilityText));
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
        _search = Keyword;
        await SearchAsync(Keyword);
    }

    private async Task SearchAsync(string keyword)
    {
        var requestId = ++_requestId;

        // The mediator scope shares one DbContext, so only one participant lookup may run at a time
        await _searchLock.WaitAsync();
        try
        {
            if (requestId != _requestId)
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

            var result = await Service.Send(new SearchParticipants.Query
            {
                Keyword = keyword,
                MaxResults = ParticipantLimit + 1
            });

            if (requestId != _requestId)
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
                _participantError = "Participant search failed, so participants are not shown. Please try again.";
            }

            _results = _pages
                .Where(page => page.DisplayText.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                               || page.Description.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                .Cast<SearchResult>()
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

            _searchLock.Release();
        }
    }

    private void SearchChanged(string? value) => _search = value ?? string.Empty;

    private async Task OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key != "Enter")
        {
            return;
        }

        await SearchAsync(_search.Trim());
    }

    private abstract record SearchResult(string DisplayText);

    private sealed record PageResult(string DisplayText, string Href, string Section, string Description) : SearchResult(DisplayText);

    private sealed record ParticipantResult(ParticipantSearchResultDto Participant)
        : SearchResult(Participant.FullName);
}
