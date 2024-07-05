using Cfo.Cats.Application.Common.Interfaces.MultiTenant;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class ApplicationUserDtoValidator : AbstractValidator<ApplicationUserDto>
{
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public ApplicationUserDtoValidator(
        IStringLocalizer<ApplicationUserDtoValidator> localizer,
        ITenantService tenantsService)
    {
        _localizer = localizer;

        RuleFor(v => v.TenantId)
            .MaximumLength(128).WithMessage(_localizer["Tenant id must be less than 128 characters"])
            .NotEmpty().WithMessage(_localizer["Tenant name cannot be empty"]);

        RuleFor(v => v.ProviderId)
            .MaximumLength(128).WithMessage(_localizer["Provider must be less than 100 characters"])
            .NotEmpty().WithMessage(_localizer["Provider cannot be empty"]);

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_localizer["User name cannot be empty"])
            .Length(2, 100).WithMessage(_localizer["User name must be between 2 and 100 characters"]);

        RuleFor(x => x.UserName)
            .EmailAddress()
            .WithMessage(_localizer["User name should be an email address"]);
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizer["E-mail cannot be empty"])
            .MaximumLength(100).WithMessage(_localizer["E-mail must be less than 100 characters"])
            .EmailAddress().WithMessage(_localizer["E-mail must be a valid email address"])
            .Must((user, email) =>
            {
                var domain = $"@{email.Split('@').LastOrDefault()}";

                var domains = tenantsService.DataSource
                    .Where(x => x.Id.Equals(user.TenantId))
                    .SelectMany(x => x.Domains);

                return domains.Contains(domain);
            }).WithMessage("Email must have a valid domain");

        RuleFor(x => x.DisplayName)
            .MaximumLength(128).WithMessage(_localizer["Display name must be less than 128 characters"]);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage(_localizer["Phone number must be less than 20 digits"]);

        _localizer = localizer;
    }
}
