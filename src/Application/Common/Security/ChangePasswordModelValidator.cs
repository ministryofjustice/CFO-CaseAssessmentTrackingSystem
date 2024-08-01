using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Security;

public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
{
    private readonly IIdentitySettings identitySettings;
    private readonly IStringLocalizer<ApplicationUserDtoValidator> localizer;

    public ChangePasswordModelValidator(
        IIdentitySettings identitySettings,
        IStringLocalizer<ApplicationUserDtoValidator> localizer
    )
    {
        this.identitySettings = identitySettings;
        this.localizer = localizer;
        RuleFor(p => p.NewPassword)
            .NotEmpty()
            .WithMessage(this.localizer["New password is required"])
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

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage(this.localizer["Confirm password must match the new password"]);
    }
}