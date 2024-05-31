using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Faker;

namespace Cfo.Cats.Server.UI.Pages.Enrolments.Search;

public partial class CandidateSearch : ComponentBase
{

    [CascadingParameter]
    public Result<CandidateDto>? CandidateResult { get; set; }
    
    [CascadingParameter]
    private UserProfile? UserProfile { get; set; }

    public bool HasResult => CandidateResult != null;

    public MudForm? form { get; set; }

    CandidateFluentValidator candidateValidator = new();

    public CandidateDto Criteria { get; set; } = new()
    {
        Identifier = Identification.UkNationalInsuranceNumber(),
        FirstName = "John",
        LastName = "Doe",
        DateOfBirth = DateTime.Now.AddYears(-18).AddDays(-1),
    };
    

    public async Task<CandidateDto[]> Submit()
    {
        CandidateSearchQuery query = new CandidateSearchQuery()
        {
            CurrentUser = UserProfile!,
            FirstName = Criteria.FirstName,
            LastName = Criteria.LastName,
            ExternalIdentifier = Criteria.Identifier,
            DateOfBirth = Criteria.DateOfBirth!.Value
        };


        var result = await Mediator.Send(query); // CandidateService.Find(Criteria.Identifier, Criteria.FirstName, Criteria.LastName, Criteria.DateOfBirth);
        return result.ToArray();
    }

    public Func<string, string?> ValidateValue(string? value) => (model) =>
    {
        var valid = value?.Equals(model, StringComparison.OrdinalIgnoreCase) ?? false;
        return valid ? null : "A partial match was found";
    };

}
