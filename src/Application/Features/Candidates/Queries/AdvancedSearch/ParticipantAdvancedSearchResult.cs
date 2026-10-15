namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchResult
{
    public required ParticipantAdvancedSearchDto[] Results { get; init; }
    public bool DmsSearched { get; init; }
    public string? Warning { get; init; }

    public bool HasWarning => !string.IsNullOrWhiteSpace(Warning);
}
