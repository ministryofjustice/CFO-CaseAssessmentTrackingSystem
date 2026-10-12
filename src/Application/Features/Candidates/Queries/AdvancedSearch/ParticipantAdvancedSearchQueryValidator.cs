using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchQueryValidator : AbstractValidator<ParticipantAdvancedSearchQuery>
{
    public ParticipantAdvancedSearchQueryValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identifier is required")
            .MaximumLength(20)
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"));
    }
}
