using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Common.Security;

public class UserProfileEditValidator : AbstractValidator<UserProfile>
{
    public UserProfileEditValidator(IStringLocalizer<ApplicationUserDtoValidator> localizer)
    {
        IStringLocalizer<ApplicationUserDtoValidator> localizer1 = localizer;
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage(localizer1["User name is required"])
            .Length(2, 100)
            .WithMessage(localizer1["User name must be between 2 and 100 characters"]);
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(localizer1["E-mail is required"])
            .MaximumLength(100)
            .WithMessage(localizer1["E-mail must be less than 100 characters"])
            .EmailAddress()
            .WithMessage(localizer1["E-mail must be a valid email address"]);

        RuleFor(x => x.DisplayName)
            .MaximumLength(128)
            .WithMessage(localizer1["Display name must be less than 128 characters"]);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage(localizer1["Phone number must be less than 20 digits"]);
    }
}