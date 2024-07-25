using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Candidates.Queries.Search;

public class CandidateSearchQueryValidator : AbstractValidator<CandidateSearchQuery>
{
    public CandidateSearchQueryValidator()
    {
        RuleFor(q => q.FirstName)
            .NotNull()
            .MinimumLength(1)
            .MaximumLength(50)
            .WithMessage("Firstname is required")
            .Matches(ValidationConstants.NameCompliantWithDMS)
            .WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "First Name"));

        RuleFor(q => q.LastName)
            .NotNull()
            .MinimumLength(1)
            .MaximumLength(50)
            .WithMessage("Lastname is required")
            .Matches(ValidationConstants.NameCompliantWithDMS)
            .WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "Last Name"));

        RuleFor(q => q.DateOfBirth)
            .NotNull()
            .WithMessage("Date of birth is required");

        RuleFor(q => q.ExternalIdentifier)
            .NotEmpty()
            .NotNull()
            .WithMessage("External identifier is required")
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "External Identifier"));

    }
}