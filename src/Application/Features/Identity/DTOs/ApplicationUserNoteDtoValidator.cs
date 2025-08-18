using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class ApplicationUserNoteDtoValidator : AbstractValidator<ApplicationUserNoteDto>
{
    public ApplicationUserNoteDtoValidator(IStringLocalizer<ApplicationUserNoteDtoValidator> localizer)
    {
        RuleFor(v => v.ApplicationUserId)
            .NotEmpty().WithMessage(localizer["User Id is required"]);

        RuleFor(v => v.CallReference)
            .MaximumLength(20).WithMessage(localizer["Call Reference must be less than or equal to 20 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Call Reference")])
            .NotEmpty().WithMessage(localizer["Call Reference is required"]);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(localizer["Message is required"])
            .MaximumLength(255).WithMessage(localizer["Message must be less than or equal to 255 characters"])
            .Matches(ValidationConstants.Notes).WithMessage(localizer[string.Format(ValidationConstants.NotesMessage, "Message")]);
    }
}
