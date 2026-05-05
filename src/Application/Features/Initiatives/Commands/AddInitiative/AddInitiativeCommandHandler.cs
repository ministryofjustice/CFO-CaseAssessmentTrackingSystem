using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.AddInitiative;

public class AddInitiativeCommandHandler(IInitiativeRepository repository)
    : IRequestHandler<AddInitiativeCommand, Result>
{
    public async Task<Result> Handle(AddInitiativeCommand request, CancellationToken cancellationToken)
    {
        var fund = Initiative.Create(
            request.Code,
            request.Description,
            request.Contract!.Id,
            request.StartDate!.Value,
            request.EndDate!.Value);

        await repository.AddAsync(fund);
        return Result.Success();
    }
}
