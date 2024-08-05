using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class ApplicationRoleDtoValidator : AbstractValidator<ApplicationRoleDto>
{
    private readonly IStringLocalizer<ApplicationRoleDtoValidator> _localizer;

    public ApplicationRoleDtoValidator(
        IStringLocalizer<ApplicationRoleDtoValidator> localizer,
        ITenantService tenantsService)
    {
        _localizer = localizer;

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage(_localizer["Description is required"])
            .MaximumLength(200).WithMessage(_localizer["Description must be fewer than 200 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Description")]);

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_localizer["Role name is required"])
            .MaximumLength(50).WithMessage(_localizer["Role name must be fewer than 50 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Role name")]);

        _localizer = localizer;
    }
}