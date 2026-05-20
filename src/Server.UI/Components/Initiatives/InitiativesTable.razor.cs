using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Initiatives.Commands.AddInitiative;
using Cfo.Cats.Application.Features.Initiatives.Commands.AmendInitiativeLifetime;
using Cfo.Cats.Application.Features.Initiatives.Commands.DeactivateInitiative;
using Cfo.Cats.Application.Features.Initiatives.Commands.EditInitiative;
using Cfo.Cats.Application.Features.Initiatives.Commands.Export;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.Initiatives.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Components.Initiatives;

public partial class InitiativesTable
{
    [Inject] private IContractService ContractService { get; set; } = null!;

    private bool _showExpired;

    protected override IRequest<Result<InitiativeDto[]>> CreateQuery()
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

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<AddInitiativeDialog>("Add Initiative", parameters, options);
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

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<EditInitiativeDialog>("Edit Initiative", parameters, options);
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

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<AmendInitiativeLifetimeDialog>("Amend Lifetime", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnDeactivate(InitiativeDto context)
    {
        var parameters = new DialogParameters<ConfirmationDialog>
        {
            { x => x.ContentText, $"This will immediately end '{context.Code} — {context.Description}'. Are you sure?" }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<ConfirmationDialog>("Deactivate Initiative", parameters, options);
        var dialogResult = await result.Result;

        if (dialogResult!.Canceled)
        {
            return;
        }

        var deactivateResult = await Service.Send(new DeactivateInitiativeCommand { Id = context.Id });

        if (deactivateResult.Succeeded)
        {
            Snackbar.Add("Initiative deactivated.", Severity.Info);
            await RefreshAsync();
        }
        else
        {
            Snackbar.Add(deactivateResult.ErrorMessage, Severity.Error);
        }
    }
}