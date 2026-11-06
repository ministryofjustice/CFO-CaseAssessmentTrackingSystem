namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

/// <summary>
/// Last participant advanced search criteria. Held in session storage (rather than the URL)
/// so the last name and date of birth are not exposed in browser history or request logs.
/// </summary>
public record AdvancedSearchSessionData
{
    public string? Identifier { get; init; }
    public string? LastName { get; init; }
    public DateTime? DateOfBirth { get; init; }
}

