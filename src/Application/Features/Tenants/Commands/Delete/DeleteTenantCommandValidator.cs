namespace Cfo.Cats.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().ForEach(v => v.NotEmpty());
    }
}
