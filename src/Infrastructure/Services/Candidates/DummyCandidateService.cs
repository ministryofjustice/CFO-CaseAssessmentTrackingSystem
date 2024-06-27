using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Matching.Core.Search;


namespace Cfo.Cats.Infrastructure.Services.Candidates;

public class DummyCandidateService : ICandidateService
{
    IReadOnlyList<CandidateDto> Candidates =>
    [
        new CandidateDto
        {
            Identifier = "1CFG1789A",
            FirstName = "Bruce",
            LastName = "Wayne",
            DateOfBirth = new DateTime(1970, 2, 19),
            Gender = "Male",
            Crn = "B001111",
            NomisNumber = "A0001AA",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG2934F",
            FirstName = "Peter",
            LastName = "Parker",
            DateOfBirth = new DateTime(2001, 8, 10),
            Gender = "Male",
            Crn = "B002222",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG3492J",
            FirstName = "Tony",
            LastName = "Stark",
            DateOfBirth = new DateTime(1970, 5, 29),
            Gender = "Male",
            Crn = "B003333",
            NomisNumber = "A0002BB",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG4567B",
            FirstName = "Steve",
            LastName = "Rogers",
            DateOfBirth = new DateTime(1918, 7, 4),
            Gender = "Male",
            Crn = "B004444",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG6789L",
            FirstName = "Clark",
            LastName = "Kent",
            DateOfBirth = new DateTime(1938, 6, 18),
            Gender = "Male",
            Crn = "B006666",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG7890M",
            FirstName = "Barry",
            LastName = "Allen",
            DateOfBirth = new DateTime(1989, 3, 14),
            Gender = "Male",
            Crn = "B007777",
            NomisNumber = "A0004DD",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG8901N",
            FirstName = "Arthur",
            LastName = "Curry",
            DateOfBirth = new DateTime(1986, 1, 29),
            Gender = "Male",
            Crn = "B008888",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG9012O",
            FirstName = "Natasha",
            LastName = "Romanoff",
            DateOfBirth = new DateTime(1984, 11, 22),
            Gender = "Female",
            Crn = "B009999",
            NomisNumber = "A0005EE",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG0123P",
            FirstName = "Wade",
            LastName = "Wilson",
            DateOfBirth = new DateTime(1974, 4, 10),
            Gender = "Male",
            Crn = "B001010",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG2345Q",
            FirstName = "Scott",
            LastName = "Lang",
            DateOfBirth = new DateTime(1968, 8, 16),
            Gender = "Male",
            Crn = "B001111",
            NomisNumber = "A0006FF",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG3456R",
            FirstName = "Stephen",
            LastName = "Strange",
            DateOfBirth = new DateTime(1930, 11, 18),
            Gender = "Male",
            Crn = "B001212",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG5678T",
            FirstName = "Bruce",
            LastName = "Banner",
            DateOfBirth = new DateTime(1969, 12, 18),
            Gender = "Male",
            Crn = "B001414",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG7890V",
            FirstName = "Carol",
            LastName = "Danvers",
            DateOfBirth = new DateTime(1984, 5, 29),
            Gender = "Female",
            Crn = "B001616",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG8901W",
            FirstName = "Matt",
            LastName = "Murdock",
            DateOfBirth = new DateTime(1979, 12, 15),
            Gender = "Male",
            Crn = "B001717",
            NomisNumber = "A0009II",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG9012X",
            FirstName = "Jessica",
            LastName = "Jones",
            DateOfBirth = new DateTime(1985, 7, 5),
            Gender = "Female",
            Crn = "B001818",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        },
        new CandidateDto
        {
            Identifier = "1CFG0123Y",
            FirstName = "Dick",
            LastName = "Grayson",
            DateOfBirth = new DateTime(1990, 3, 20),
            Gender = "Male",
            Crn = "B001919",
            NomisNumber = "A0010JJ",
            Nationality = "British",
            Ethnicity = "",
            Origin = "Nomis",
            CurrentLocation = "Risley",
        }
    ];

    public async Task<CandidateDto?> GetByUpciAsync(string upci)
    {
        var candidate = Candidates.SingleOrDefault(c => c.Identifier == upci);
        return await Task.FromResult(candidate);
    }

    public async Task<IEnumerable<SearchResult>?> SearchAsync(CandidateSearchQuery searchQuery)
    {
        string lastName = searchQuery.LastName;
        string identifier = searchQuery.ExternalIdentifier;
        DateOnly dateOfBirth = DateOnly.FromDateTime((DateTime)searchQuery.DateOfBirth!);

        var blocks = Candidates
            .Where(e => e.LastName == searchQuery.LastName && e.DateOfBirth == searchQuery.DateOfBirth)
            .Union
            (
                Candidates.Where(e => new[] { e.Crn, e.NomisNumber }.Contains(searchQuery.ExternalIdentifier))
            );

        if(blocks.Count() is 0)
        {
            return [];
        }

        var scores = blocks.Select(block => Score((identifier, lastName, dateOfBirth), block.Crn ?? string.Empty, block))
            .Union
            (
                blocks.Select(block => Score((identifier, lastName, dateOfBirth), block.NomisNumber ?? string.Empty, block))
            )
            .GroupBy(result => result.Upci)
            .Select(result => new SearchResult(result.Key, result.Min(r => r.Precedence)));

        return await Task.FromResult(scores);
    }

    static SearchResult Score((string externalIdentifier, string lastName, DateOnly dateOfBirth) query, string identifier, CandidateDto block) => 
        new(block.Identifier, Precedence.GetPrecedence
        (
            identifiers:
            (
                query.externalIdentifier,
                identifier
            ),
            lastNames:
            (
                query.lastName,
                block.LastName
            ),
            dateOfBirths:
            (
                query.dateOfBirth,
                DateOnly.FromDateTime(block.DateOfBirth)
            )
        ));

}
