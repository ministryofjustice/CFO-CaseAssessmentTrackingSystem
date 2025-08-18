using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class ApplicationRoleDtoValidator : AbstractValidator<ApplicationRoleDto>
{
    public ApplicationRoleDtoValidator(IStringLocalizer<ApplicationRoleDtoValidator> localizer)
    {

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage(localizer["Description is required"])
            .MaximumLength(200).WithMessage(localizer["Description must be fewer than 200 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Description")]);

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(localizer["Role name is required"])
            .MaximumLength(50).WithMessage(localizer["Role name must be fewer than 50 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Role name")]);

    }
}