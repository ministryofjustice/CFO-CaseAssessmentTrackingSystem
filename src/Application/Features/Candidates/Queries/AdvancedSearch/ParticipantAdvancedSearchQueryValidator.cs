using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchQueryValidator : AbstractValidator<ParticipantAdvancedSearchQuery>
{
    private const int ExternalIdentifierLength = 7;
    private const int SearchJustificationLength = ValidationConstants.PicklistTextLength;

    public ParticipantAdvancedSearchQueryValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identifier is required")
            .MaximumLength(9)
            .WithMessage("Identifier must be 9 characters or fewer")
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"))
            .Must(IdentifierHasRecognisedFormat)
            .WithMessage("Identifier must be a 7 character external identifier or a 9 character CATS Id");

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .WithMessage("Last name must be 50 characters or fewer");

        RuleFor(x => x.LastName)
            .Matches(ValidationConstants.NameCompliantWithDMS)
            .WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "Last Name"))
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required when date of birth is provided")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.DateOfBirth)
            .NotNull()
            .WithMessage("Date of birth is required when last name is provided")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(_ => DateTime.Today.AddYears(-18))
            .WithMessage("Must be at least 18 years")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.Justification)
            .NotEmpty()
            .WithMessage("Search justification is required")
            .MaximumLength(SearchJustificationLength)
            .WithMessage($"Search justification must be less than or equal to {SearchJustificationLength} characters")
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Search justification"));
    }

    private static bool IdentifierHasRecognisedFormat(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return false;
        }

        var length = identifier.Trim().Length;
        return length is ExternalIdentifierLength or ValidationConstants.ParticipantIdLength;
    }
}
