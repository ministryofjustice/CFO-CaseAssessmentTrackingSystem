using Cfo.Cats.Application.Common.Validators;
namespace Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommandValidator : AbstractValidator<AddEditTenantCommand>
{
    public AddEditTenantCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(50)
            .Matches(ValidationConstants.LettersSpacesUnderscores)
            .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Name"))
            .NotEmpty();
        
        RuleFor(v => v.Description)
            .Matches(ValidationConstants.LettersSpacesUnderscores)
            .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Description"))
            .MaximumLength(150).NotEmpty();

        RuleFor(v => v.Id)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Tenant Id is required and must be less than 50 characters");

        RuleFor(v => v.Id)
            .Matches(ValidationConstants.TenantId)
            .WithMessage(ValidationConstants.TenantIdMessage);
    }
}
