using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Labels.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Components.Labels;

public partial class LabelsTable
{
    protected override IRequest<Result<LabelDto[]>> CreateQuery()
        => new GetVisibleLabels.Query(CurrentUser);

    private async Task OnAddLabel()
    {
        AddLabelCommand command = new() 
        {
            Scope = LabelScope.User,
            Colour = AppColour.Default,
            Variant = AppVariant.Filled,
            Name = string.Empty,
            Description = string.Empty,
            ContractId = null
        };

        var parameters = new DialogParameters<AddLabelDialog>()
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };

        var result = await DialogService.ShowAsync<AddLabelDialog>("Add Label", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnEdit(LabelDto context)
    {
        var command = new EditLabelCommand()
        {
            NewColour = context.Colour,
            NewName = context.Name,
            NewDescription = context.Description,
            LabelId = new LabelId(context.Id),
            NewVariant = context.Variant,
            NewScope =  context.Scope,
            NewAppIcon = context.AppIcon
        };

        var parameters = new DialogParameters<EditLabelDialog>()
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<EditLabelDialog>("Edit Label", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnDelete(LabelDto context)
    {
        var command = new DeleteLabelCommand()
        {
            LabelId = new LabelId(context.Id),
            UserProfile = CurrentUser,
        };

        var label = context.Contract switch
        {
            null => $"Are you sure you want to delete the global label {context.Name}?",
            _ => $"Are you sure you want to delete the {context.Name} label from the {context.Contract} contract?",
        };
        
        var parameters = new DialogParameters<ConfirmationDialog>()
        {
            { x => x.ContentText, label },
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<ConfirmationDialog>(@ConstantString.DeleteHeader, parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            var deleteResult = await Service.Send(command);
            if (deleteResult.Succeeded)
            {
                await RefreshAsync();    
            }
            else
            {
                Snackbar.Add(deleteResult.ErrorMessage, Severity.Error);
            }
        }
    }
}