using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Security;

public class ResetPasswordFormModelValidator : AbstractValidator<ResetPasswordFormModel>
{
    private readonly IStringLocalizer<ApplicationUserDtoValidator> localizer;
    private readonly IIdentitySettings identitySettings;

    public ResetPasswordFormModelValidator(
        IStringLocalizer<ApplicationUserDtoValidator> localizer,
        IIdentitySettings identitySettings
    )
    {
        this.localizer = localizer;
        this.identitySettings = identitySettings;

        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage(this.localizer["Password is required"])
            .MinimumLength(this.identitySettings.RequiredLength)
            .WithMessage(
            this.localizer[
                    "Password must be at least {0} characters long",
                    this.identitySettings.RequiredLength
                ]
            )
            .MaximumLength(this.identitySettings.MaxLength)
            .WithMessage(
            this.localizer[
                    "Password cannot be longer than {0} characters",
                    this.identitySettings.MaxLength
                ]
            )
            .Matches(this.identitySettings.RequireUpperCase ? @"[A-Z]+" : string.Empty)
            .WithMessage(this.localizer["Password must contain at least one uppercase letter"])
            .Matches(this.identitySettings.RequireLowerCase ? @"[a-z]+" : string.Empty)
            .WithMessage(this.localizer["Password must contain at least one lowercase letter"])
            .Matches(this.identitySettings.RequireDigit ? @"[0-9]+" : string.Empty)
            .WithMessage(this.localizer["Password must contain at least one digit"])
            .Matches(this.identitySettings.RequireNonAlphanumeric ? @"[\@\!\?\*\.]+" : string.Empty)
            .WithMessage(
            this.localizer[
                    "Password must contain at least one non-alphanumeric character (e.g., @, !, ?, *, .)"
                ]
            );
    }
}