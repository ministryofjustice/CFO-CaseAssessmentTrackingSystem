using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.DeactivateInnovationFund;

public class DeactivateInnovationFundCommandHandler(IInnovationFundRepository repository)
    : IRequestHandler<DeactivateInnovationFundCommand, Result>
{
    public async Task<Result> Handle(DeactivateInnovationFundCommand request, CancellationToken cancellationToken)
    {
        var fund = await repository.GetByIdAsync(request.Id);
        fund.Deactivate();
        return Result.Success();
    }
}
