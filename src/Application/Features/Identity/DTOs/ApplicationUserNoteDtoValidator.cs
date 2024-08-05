
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class ApplicationUserNoteDtoValidator : AbstractValidator<ApplicationUserNoteDto>
{
    private readonly IStringLocalizer<ApplicationUserNoteDtoValidator> _localizer;

    public ApplicationUserNoteDtoValidator(
        IStringLocalizer<ApplicationUserNoteDtoValidator> localizer,
        ITenantService tenantsService)
    {
        _localizer = localizer;

        RuleFor(v => v.ApplicationUserId)
            .NotEmpty().WithMessage(_localizer["User Id is required"]);

        RuleFor(v => v.CallReference)
            .MaximumLength(20).WithMessage(_localizer["Call Reference must be less than or equal to 20 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Call Reference")])
            .NotEmpty().WithMessage(_localizer["Call Reference is required"]);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(_localizer["Message is required"])
            .MaximumLength(255).WithMessage(_localizer["Message must be less than or equal to 255 characters"])
            .Matches(ValidationConstants.Notes).WithMessage(_localizer[string.Format(ValidationConstants.NotesMessage, "Message")]);

        _localizer = localizer;
    }
}
