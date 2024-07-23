namespace Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommandValidator : AbstractValidator<AddEditTenantCommand>
{
    public AddEditTenantCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(50)
            .Matches(@"^[A-Za-z_ ]+$")
            .WithMessage("Name must contain only letters, spaces, and underscores.")
            .NotEmpty();
        
        RuleFor(v => v.Description)
            .Matches(@"^[A-Za-z_ ]+$")
            .WithMessage("Name must contain only letters, spaces, and underscores.")
            .MaximumLength(150).NotEmpty();

        RuleFor(v => v.Id)
            .MaximumLength(50).NotEmpty();

        RuleFor(v => v.Id)
            .Matches("^[0-9]+(?:\\.[0-9]+)*\\.$")
            .WithMessage("Must start with a number and end with a period");
    }
}
