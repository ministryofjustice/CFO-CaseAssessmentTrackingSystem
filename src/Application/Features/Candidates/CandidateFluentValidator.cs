using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Common.Validators;
namespace Cfo.Cats.Application.Features.Candidates;

public class CandidateFluentValidator : AbstractValidator<CandidateDto>
{
    public CandidateFluentValidator()
    {
        RuleFor(x => x.Identifier)
            .MinimumLength(8)
            .MaximumLength(9)
            .NotEmpty()
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Identifier"));

        RuleFor(x => x.FirstName)
            .MinimumLength(2)
            .MaximumLength(50)
            .NotEmpty()
            .Matches(ValidationConstants.NameCompliantWithDMS).WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "First name"));

        RuleFor(x => x.LastName)
            .MinimumLength(2)
            .MaximumLength(50)
            .NotEmpty()
            .Matches(ValidationConstants.NameCompliantWithDMS).WithMessage(string.Format(ValidationConstants.NameCompliantWithDMSMessage, "Last name"));

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidAge)
            .WithMessage("Must be at least 18 years")
            .NotEmpty();
    }
    
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<CandidateDto>.CreateWithOptions((CandidateDto)model, x => x.IncludeProperties(propertyName)));

        if (result.IsValid)
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select(e => e.ErrorMessage);
    };

    private bool BeValidAge(DateTime date)
    {
        var minimumAge = DateTime.Now.Date.AddYears(-18);
        return (minimumAge < date) == false;
    }

}