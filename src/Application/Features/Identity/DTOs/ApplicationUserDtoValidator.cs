using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class ApplicationUserDtoValidator : AbstractValidator<ApplicationUserDto>
{
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public ApplicationUserDtoValidator(
        IStringLocalizer<ApplicationUserDtoValidator> localizer,
        ITenantService tenantsService)
    {
        _localizer = localizer;

        RuleFor(x => x.TenantId)
            .MaximumLength(50).WithMessage(_localizer["Tenant id must be less than 50 characters"])
            .NotEmpty().WithMessage(_localizer["Tenant name is required"])
            .Matches(ValidationConstants.TenantId).WithMessage(_localizer[ValidationConstants.TenantIdMessage]);

        RuleFor(x => x.ProviderId)
            .MaximumLength(50).WithMessage(_localizer["Provider must be less than 50 characters"])
            .NotEmpty().WithMessage(_localizer["Provider is required"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Provider Id")]);

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage(_localizer["Display Name is required and must be less than or equal to 100 characters"])
            .NotEmpty().WithMessage(_localizer["Display Name is required"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Display Name")]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizer["E-mail is required"])
            .Length(6, 255).WithMessage(_localizer["E-mail must be between 6 and 255 characters"])
            .EmailAddress().WithMessage(_localizer["E-mail must be a valid email address"])
            .Must((user, email) =>
            {
                var domain = $"@{email.Split('@').LastOrDefault()}";

                var domains = tenantsService.DataSource
                    .Where(x => x.Id.Equals(user.TenantId))
                    .SelectMany(x => x.Domains);

                return domains.Contains(domain);
            }).WithMessage("Email is not valid for this tenant");
        
        RuleFor(x => x.UserName)
            .Equal(x=> x.Email)
            .WithMessage(_localizer["User name must be a valid email address between 6 and 255 characters"]);


        RuleFor(x => x.UserName)
            .EmailAddress()
            .WithMessage(_localizer["User name should be an email address"]);

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage(_localizer["Display name must be less than or equal to 100 characters"])
            .NotEmpty().WithMessage(_localizer["Display name is required"]);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage(_localizer["Mobile number must be less than or equal to 20 digits"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Mobile number")]);

        RuleFor(x => x.MemorablePlace)
            .MaximumLength(50).WithMessage(_localizer["Memorable place must be less than or equal to 50 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Memorable place")]);

        RuleFor(x => x.MemorableDate)
            .NotEmpty().WithMessage(_localizer["Memorable date is required"]);

        RuleForEach(x => x.Notes).ChildRules(notes =>
        {
            notes.RuleFor(y => y.CallReference)
            .NotEmpty().WithMessage(_localizer["Call Reference is required"])
            .MaximumLength(20).WithMessage(_localizer["Call Reference must be less than or equal to 20 characters"])
            .Matches(ValidationConstants.AlphaNumeric).WithMessage(_localizer[string.Format(ValidationConstants.AlphaNumericMessage, "Call Reference")]);
        });
        RuleForEach(x => x.Notes).ChildRules(notes =>
        {
            notes.RuleFor(y => y.Message)
            .NotEmpty().WithMessage(_localizer["Message is required"])
            .MaximumLength(255).WithMessage(_localizer["Message must be less than or equal to 255 characters"])
            .Matches(ValidationConstants.Notes).WithMessage(_localizer[string.Format(ValidationConstants.NotesMessage, "Message")]);
        });

        _localizer = localizer;
    }
}
