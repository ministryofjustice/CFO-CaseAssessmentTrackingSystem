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
            .WithMessage("Firstname is required");
        
        RuleFor(q => q.FirstName)
            .Matches(RegularExpressionValidation.NameCompliantWithDMS)
            .WithMessage(string.Format(RegularExpressionValidation.NameCompliantWithDMSMessage, "First Name"));

        RuleFor(q => q.LastName)
            .NotNull()
            .MinimumLength(1)
            .MaximumLength(50)
            .WithMessage("Lastname is required");
        
        RuleFor(q => q.LastName)
            .Matches(RegularExpressionValidation.NameCompliantWithDMS)
            .WithMessage(string.Format(RegularExpressionValidation.NameCompliantWithDMSMessage, "Last Name"));

        RuleFor(q => q.DateOfBirth)
            .NotNull()
            .WithMessage("Date of birth is required");

        RuleFor(q => q.DateOfBirth.ToString())
            .Matches(RegularExpressionValidation.Date)
            .WithMessage(string.Format(RegularExpressionValidation.DateMessage, "Date of birth"));

        RuleFor(q => q.ExternalIdentifier)
            .NotEmpty()
            .NotNull()
            .WithMessage("External identifier is required");

        RuleFor(q => q.ExternalIdentifier)
            .Matches(RegularExpressionValidation.AlphaNumeric)
            .WithMessage(string.Format(RegularExpressionValidation.AlphaNumeric, "External Identifier"));
    }
}