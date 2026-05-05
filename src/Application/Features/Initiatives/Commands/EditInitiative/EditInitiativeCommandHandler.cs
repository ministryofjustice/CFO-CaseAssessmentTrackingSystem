using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.EditInitiative;

public class EditInitiativeCommandHandler(IInitiativeRepository repository)
    : IRequestHandler<EditInitiativeCommand, Result>
{
    public async Task<Result> Handle(EditInitiativeCommand request, CancellationToken cancellationToken)
    {
        var fund = await repository.GetByIdAsync(request.Id);
        fund.Edit(request.Code, request.Description, request.Contract!.Id);
        return Result.Success();
    }
}
