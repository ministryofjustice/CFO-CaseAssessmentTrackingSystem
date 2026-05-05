using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.AddInnovationFund;

public class AddInnovationFundCommandHandler(IInnovationFundRepository repository)
    : IRequestHandler<AddInnovationFundCommand, Result>
{
    public async Task<Result> Handle(AddInnovationFundCommand request, CancellationToken cancellationToken)
    {
        var fund = InnovationFund.Create(
            request.Code,
            request.Description,
            request.Contract!.Id,
            request.StartDate!.Value,
            request.EndDate!.Value);

        await repository.AddAsync(fund);
        return Result.Success();
    }
}
