namespace Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommandValidator : AbstractValidator<AddEditTenantCommand>
{
    public AddEditTenantCommandValidator()
    {
        RuleFor(v => v.Name).MaximumLength(256).NotEmpty();

        RuleFor(v => v.Description).MaximumLength(150).NotEmpty();

        RuleFor(v => v.Id).MaximumLength(200).NotEmpty();

        RuleFor(v => v.Id)
            .Matches("^[0-9]+(?:\\.[0-9]+)*\\.$")
            .WithMessage("Must start with a number and end with a period");
    }
}
