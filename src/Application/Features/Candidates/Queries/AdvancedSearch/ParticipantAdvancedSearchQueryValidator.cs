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
            .WithMessage("Identifier must be 9 characters or fewer")
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"));

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .WithMessage("Last name must be 50 characters or fewer");

        RuleFor(x => x.LastName)
            .Matches(ValidationConstants.NameCompliantWithDMS)
            .WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "Last Name"))
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));
    }
}
