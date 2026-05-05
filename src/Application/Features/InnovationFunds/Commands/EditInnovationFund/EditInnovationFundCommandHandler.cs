using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.EditInnovationFund;

public class EditInnovationFundCommandHandler(IInnovationFundRepository repository)
    : IRequestHandler<EditInnovationFundCommand, Result>
{
    public async Task<Result> Handle(EditInnovationFundCommand request, CancellationToken cancellationToken)
    {
        var fund = await repository.GetByIdAsync(request.Id);
        fund.Edit(request.Code, request.Description, request.Contract!.Id, request.StartDate!.Value, request.EndDate!.Value);
        return Result.Success();
    }
}
