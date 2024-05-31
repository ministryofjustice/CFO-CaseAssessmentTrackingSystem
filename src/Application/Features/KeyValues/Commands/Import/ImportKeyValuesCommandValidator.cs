#nullable disable warnings
namespace Cfo.Cats.Application.Features.KeyValues.Commands.Import;

public class ImportKeyValuesCommandValidator : AbstractValidator<ImportKeyValuesCommand>
{
    public ImportKeyValuesCommandValidator()
    {
        RuleFor(x => x.Data).NotNull().NotEmpty();
    }
}
