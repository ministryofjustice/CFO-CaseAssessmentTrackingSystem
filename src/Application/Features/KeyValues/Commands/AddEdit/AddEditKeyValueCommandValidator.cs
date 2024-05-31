namespace Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandValidator : AbstractValidator<AddEditKeyValueCommand>
{
    public AddEditKeyValueCommandValidator()
    {
        RuleFor(v => v.Name).NotNull();
        RuleFor(v => v.Text).MaximumLength(256).NotEmpty();
        RuleFor(v => v.Value).MaximumLength(256).NotEmpty();
    }
}
