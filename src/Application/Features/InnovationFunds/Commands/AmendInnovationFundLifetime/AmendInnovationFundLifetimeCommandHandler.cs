using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.AmendInnovationFundLifetime;

public class AmendInnovationFundLifetimeCommandHandler(IInnovationFundRepository repository)
    : IRequestHandler<AmendInnovationFundLifetimeCommand, Result>
{
    public async Task<Result> Handle(AmendInnovationFundLifetimeCommand request, CancellationToken cancellationToken)
    {
        var fund = await repository.GetByIdAsync(request.Id);
        fund.AmendLifetime(request.StartDate!.Value, request.EndDate!.Value);
        return Result.Success();
    }
}
