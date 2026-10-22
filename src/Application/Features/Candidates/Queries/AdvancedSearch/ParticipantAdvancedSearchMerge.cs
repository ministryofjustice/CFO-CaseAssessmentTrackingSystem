using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public static class ParticipantAdvancedSearchMerge
{
    public static List<string> SelectDmsUpcis(
        IEnumerable<SearchResult> searchResults,
        IReadOnlySet<string> catsIdentifiers,
        string identifier,
        int maximumResults)
    {
        var upcis = searchResults
            .OrderBy(r => r.Precedence)
            .Select(r => r.Upci)
            .Distinct()
            .Where(upci => !catsIdentifiers.Contains(upci))
            .Take(maximumResults)
            .ToList();

        if (upcis.Count == 0
            && catsIdentifiers.Count == 0
            && identifier.Length == ValidationConstants.ParticipantIdLength)
        {
            upcis.Add(identifier);
        }

        return upcis;
    }
}
