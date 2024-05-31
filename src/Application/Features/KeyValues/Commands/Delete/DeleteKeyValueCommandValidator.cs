namespace Cfo.Cats.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommandValidator : AbstractValidator<DeleteKeyValueCommand>
{
    public DeleteKeyValueCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
