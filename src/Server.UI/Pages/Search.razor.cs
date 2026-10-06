using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Pages;

public partial class Search
{
    [Parameter] public string Keyword { get; set; } = string.Empty;

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = null!;

    private readonly List<PageResult> _pages = [];
    private List<SearchResult> _results = [];
    private string _search = string.Empty;
    private string _searchedKeyword = string.Empty;
    private bool _loading;

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
                        _pages.Add(new PageResult(link.DisplayText, href));
                        break;
                    case NavigationMenuItemButtonModel { Href: { Length: > 0 } href } button:
                        _pages.Add(new PageResult(button.DisplayText, href));
                        break;
                }
            }
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        _search = Keyword;
        await SearchAsync(Keyword);
    }

    private async Task SearchAsync(string keyword)
    {
        try
        {
            _searchedKeyword = keyword;
            _loading = true;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                _results = [];
            }
            else
            {
                var result = await Service.Send(new SearchParticipants.Query
                {
                    Keyword = keyword,
                    CurrentUser = CurrentUser
                });

                var participants = result is { Succeeded: true, Data: not null }
                    ? result.Data.Select(participant => new ParticipantResult(participant))
                    : [];

                _results = _pages
                    .Where(page => page.DisplayText.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                    .Cast<SearchResult>()
                    .Concat(participants)
                    .OrderBy(searchResult => searchResult.DisplayText)
                    .ToList();
            }
        }
        finally
        {
            _loading = false;
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

    private sealed record PageResult(string DisplayText, string Href) : SearchResult(DisplayText);

    private sealed record ParticipantResult(ParticipantSearchResultDto Participant)
        : SearchResult(Participant.FullName);
}
