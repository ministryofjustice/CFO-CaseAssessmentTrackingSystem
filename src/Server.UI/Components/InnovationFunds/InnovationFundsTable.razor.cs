using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.InnovationFunds.Commands.AddInnovationFund;
using Cfo.Cats.Application.Features.InnovationFunds.Commands.EditInnovationFund;
using Cfo.Cats.Application.Features.InnovationFunds.DTOs;
using Cfo.Cats.Application.Features.InnovationFunds.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.InnovationFunds;

public partial class InnovationFundsTable
{
    [Inject] private IContractService ContractService { get; set; } = null!;

    protected override IRequest<Result<InnovationFundDto[]>> CreateQuery()
        => new GetInnovationFunds.Query();

    private async Task OnAdd()
    {
        var command = new AddInnovationFundCommand
        {
            Code = string.Empty,
            Description = string.Empty
        };

        var parameters = new DialogParameters<AddInnovationFundDialog>
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<AddInnovationFundDialog>("Add Innovation Fund", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnEdit(InnovationFundDto context)
    {
        var contract = ContractService.DataSource.FirstOrDefault(c => c.Id == context.ContractId);

        var command = new EditInnovationFundCommand
        {
            Id = context.Id,
            Code = context.Code,
            Description = context.Description,
            Contract = contract,
            StartDate = context.LifetimeStart,
            EndDate = context.LifetimeEnd
        };

        var parameters = new DialogParameters<EditInnovationFundDialog>
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<EditInnovationFundDialog>("Edit Innovation Fund", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }
}
