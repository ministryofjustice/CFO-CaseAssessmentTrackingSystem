using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchQueryValidator : AbstractValidator<ParticipantAdvancedSearchQuery>
{
    public ParticipantAdvancedSearchQueryValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identifier is required")
            .MaximumLength(9)
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"));

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .WithMessage("Last name must be 100 characters or fewer");
    }
}
