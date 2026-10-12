using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Microsoft.AspNetCore.Components.Web;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class AdvancedSearch
{
    private static readonly IReadOnlyList<BreadcrumbItem> Breadcrumbs =
    [
        ParticipantLinks.Home.AsMudBreadcrumb(),
        new("Participant Advanced Search", "/pages/workspace/participants/advanced-search")
    ];

    private string? _identifier;
    private string? _searchedIdentifier;
    private bool _searched;
    private ParticipantAdvancedSearchDto? _selected;
    private string? _warning;

    [SupplyParameterFromQuery(Name = "identifier")]
    private string? Identifier { get; set; }

    private string ParticipantHref =>
        $"/pages/workspace/participants/{_selected?.Identifier}?from=participant-advanced-search&identifier={Uri.EscapeDataString(_searchedIdentifier ?? string.Empty)}";

    // no data is loaded until the user performs a search
    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(Identifier))
        {
            _identifier = Identifier.Trim();
            await SearchAsync();
        }
        else
        {
            Loading = false;
        }
    }

    protected override IQuery<Result<ParticipantAdvancedSearchResult>> CreateQuery() => new ParticipantAdvancedSearchQuery
    {
        Identifier = _identifier?.Trim(),
        CurrentUser = CurrentUser
    };

    private async Task OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchAsync();
        }
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(_identifier))
        {
            return;
        }

        _selected = null;
        _warning = null;
        Data = null;
        _searched = true;
        _searchedIdentifier = _identifier.Trim();

        if (await LoadDataAsync() && Data is not null)
        {
            _warning = Data.Warning;
            _selected = Data.Results.Length == 1 ? Data.Results[0] : null;
        }
    }
}
