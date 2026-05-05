using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.AmendInitiativeLifetime;

public class AmendInitiativeLifetimeCommandHandler(IInitiativeRepository repository)
    : IRequestHandler<AmendInitiativeLifetimeCommand, Result>
{
    public async Task<Result> Handle(AmendInitiativeLifetimeCommand request, CancellationToken cancellationToken)
    {
        var fund = await repository.GetByIdAsync(request.Id);
        fund.AmendLifetime(request.StartDate!.Value, request.EndDate!.Value.Date.Add(new TimeSpan(23, 59, 59)));
        return Result.Success();
    }
}
