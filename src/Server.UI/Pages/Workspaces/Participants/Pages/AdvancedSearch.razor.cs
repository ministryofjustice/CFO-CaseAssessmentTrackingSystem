using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;
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
    private string _justification = string.Empty;
    private string? _searchedJustification;
    private string? _searchedIdentifier;
    private bool _searched;
    private bool _searchInProgress;
    private bool _showIdentifierError;
    private bool _showJustificationError;
    private ParticipantAdvancedSearchDto? _selected;
    private string? _warning;
    private DateTime AdultDobCutoff => DateTime.Today.AddYears(-18);

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

        Loading = false;
    }

    protected override IQuery<Result<ParticipantAdvancedSearchResult>> CreateQuery() => new ParticipantAdvancedSearchQuery
    {
        Identifier = _identifier?.Trim(),
        LastName = _lastName?.Trim(),
        DateOfBirth = _dateOfBirth,
        Justification = _justification.Trim(),
        CurrentUser = CurrentUser
    };

    private void IdentifierChanged(string? value)
    {
        _identifier = value;
        _showIdentifierError = false;
    }

    private void JustificationChanged(string? value)
    {
        _justification = value ?? string.Empty;
        _showJustificationError = false;
    }

    private async Task SearchAsync()
    {
        if (_searchInProgress)
        {
            return;
        }

        _searchInProgress = true;
        try
        {
            ErrorMessage = null;
            _showIdentifierError = false;
            _showJustificationError = false;

            if (string.IsNullOrWhiteSpace(_identifier))
            {
                _showIdentifierError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(_justification))
            {
                _showJustificationError = true;
                return;
            }

            _selected = null;
            _warning = null;
            Data = null;
            _searched = true;
            _searchedIdentifier = _identifier.Trim();
            _searchedJustification = _justification.Trim();

            await SessionStorage.SetAsync(new AdvancedSearchSessionData
            {
                Identifier = _searchedIdentifier,
                LastName = _lastName?.Trim(),
                DateOfBirth = _dateOfBirth
            });

            if (await LoadDataAsync() && Data is not null)
            {
                _warning = Data.Warning;
                _selected = Data.Results.Length == 1 ? Data.Results[0] : null;
                UpdateUrl();
            }
        }
        finally
        {
            _searchInProgress = false;
        }
    }

    /// <summary>
    /// Keeps the identifier in the URL so navigation context remains tied to the latest executed search.
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
