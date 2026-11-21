using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class AdvancedSearch
{
    private static readonly IReadOnlyList<BreadcrumbItem> Breadcrumbs =
    [
        ParticipantLinks.Home.AsMudBreadcrumb(),
        ParticipantLinks.AdvancedSearch.AsMudBreadcrumb()
    ];

    private string? _identifier;
    private string? _lastName;
    private DateTime? _dateOfBirth;
    private string? _searchedIdentifier;
    private bool _searched;
    private bool _searchInProgress;
    private ParticipantAdvancedSearchDto? _selected;
    private string? _warning;

    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    /// <summary>
    /// Only the identifier is carried in the URL (e.g. when returning via the participant breadcrumb).
    /// Last name and date of birth are restored from session storage to keep personal data out of URLs.
    /// </summary>
    [SupplyParameterFromQuery(Name = "identifier")]
    private string? Identifier { get; set; }

    private string ParticipantHref => QueryHelpers.AddQueryString(
        ParticipantLinks.Participant(_selected!.Identifier).Href,
        new Dictionary<string, string?>
        {
            ["from"] = "participant-advanced-search",
            ["identifier"] = _searchedIdentifier
        });

    // no data is loaded until the user performs a search
    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Identifier))
        {
            Loading = false;
            return;
        }

        _identifier = Identifier.Trim();

        var cached = await SessionStorage.GetAsync<AdvancedSearchSessionData>();
        if (cached is { Succeeded: true, Data: { } criteria }
            && string.Equals(criteria.Identifier, _identifier, StringComparison.OrdinalIgnoreCase))
        {
            _lastName = criteria.LastName;
            _dateOfBirth = criteria.DateOfBirth;
        }

        await SearchAsync();
    }

    protected override IQuery<Result<ParticipantAdvancedSearchResult>> CreateQuery() => new ParticipantAdvancedSearchQuery
    {
        Identifier = _identifier?.Trim(),
        LastName = _lastName?.Trim(),
        DateOfBirth = _dateOfBirth,
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
        if (_searchInProgress || string.IsNullOrWhiteSpace(_identifier))
        {
            return;
        }

        _searchInProgress = true;
        try
        {
            _selected = null;
            _warning = null;
            Data = null;
            _searched = true;
            _searchedIdentifier = _identifier.Trim();

            await SessionStorage.SetAsync(new AdvancedSearchSessionData
            {
                Identifier = _searchedIdentifier,
                LastName = _lastName?.Trim(),
                DateOfBirth = _dateOfBirth
            });

            UpdateUrl();

            if (await LoadDataAsync() && Data is not null)
            {
                _warning = Data.Warning;
                _selected = Data.Results.Length == 1 ? Data.Results[0] : null;
            }
        }
        finally
        {
            _searchInProgress = false;
        }
    }

    /// <summary>
    /// Keeps the identifier (only) in the URL so a page refresh re-runs the search.
    /// </summary>
    private void UpdateUrl()
    {
        if (string.Equals(Identifier, _searchedIdentifier, StringComparison.Ordinal))
        {
            return;
        }

        Navigation.NavigateTo(Navigation.GetUriWithQueryParameter("identifier", _searchedIdentifier), replace: true);
    }
}
