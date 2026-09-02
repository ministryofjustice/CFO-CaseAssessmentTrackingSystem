using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Initiatives.Commands.AddInitiative;
using Cfo.Cats.Application.Features.Initiatives.Commands.AmendInitiativeLifetime;
using Cfo.Cats.Application.Features.Initiatives.Commands.EditInitiative;
using Cfo.Cats.Application.Features.Initiatives.Commands.Export;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.Initiatives.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components.Initiatives;

public partial class InitiativesTable
{
    [Inject] private IContractService ContractService { get; set; } = null!;

    private bool _showExpired;

    protected override IQuery<Result<InitiativeDto[]>> CreateQuery()
        => new GetInitiatives.Query { IncludeExpired = _showExpired };

    private async Task OnExport()
    {
        try
        {
            var result = await Service.Send(new ExportInitiatives.Command
            {
                Query = new GetInitiatives.Query { IncludeExpired = _showExpired }
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error has occurred while generating the initiatives export");
            Snackbar.Add("An error has occurred while generating the initiatives export.", Severity.Error);
        }
    }

    private async Task OnAdd()
    {
        var command = new AddInitiativeCommand
        {
            Code = string.Empty,
            Description = string.Empty
        };

        var parameters = new DialogParameters<AddInitiativeDialog>
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false };
        var result = await DialogService.ShowAsync<AddInitiativeDialog>(ConstantString.AddInitiative, parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnEdit(InitiativeDto context)
    {
        var contract = ContractService.DataSource.FirstOrDefault(c => c.Id == context.ContractId);

        var command = new EditInitiativeCommand
        {
            Id = context.Id,
            Code = context.Code,
            Description = context.Description,
            Contract = contract
        };

        var parameters = new DialogParameters<EditInitiativeDialog>
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false };
        var result = await DialogService.ShowAsync<EditInitiativeDialog>(ConstantString.EditInitiative, parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnAmendLifetime(InitiativeDto context)
    {
        var command = new AmendInitiativeLifetimeCommand
        {
            Id = context.Id,
            StartDate = context.LifetimeStart,
            EndDate = context.LifetimeEnd
        };

        var parameters = new DialogParameters<AmendInitiativeLifetimeDialog>
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false };
        var result = await DialogService.ShowAsync<AmendInitiativeLifetimeDialog>(ConstantString.AmendLifetime, parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }
}
