using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.DeactivateInitiative;

public class DeactivateInitiativeCommandHandler(IInitiativeRepository repository)
    : IRequestHandler<DeactivateInitiativeCommand, Result>
{
    public async Task<Result> Handle(DeactivateInitiativeCommand request, CancellationToken cancellationToken)
    {
        var fund = await repository.GetByIdAsync(request.Id);
        fund.Deactivate();
        return Result.Success();
    }
}
