using Cfo.Cats.Application.Features.Labels.Commands.AddLabel;
using Cfo.Cats.Application.Features.Labels.Commands.DeleteLabel;
using Cfo.Cats.Application.Features.Labels.Commands.EditLabel;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Labels.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Labels;

public partial class LabelsTable
{
    private string? _searchString;
    private AppColour? _colourFilter;
    private AppVariant? _variantFilter;
    private LabelScope? _scopeFilter;

    private IEnumerable<LabelDto> FilteredData
    {
        get
        {
            if (Data is null)
            {
                return [];
            }

            IEnumerable<LabelDto> filtered = Data;

            if (string.IsNullOrWhiteSpace(_searchString) == false)
            {
                var search = _searchString.Trim();
                filtered = filtered.Where(l =>
                    l.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || l.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (_colourFilter is not null)
            {
                filtered = filtered.Where(l => l.Colour == _colourFilter);
            }

            if (_variantFilter is not null)
            {
                filtered = filtered.Where(l => l.Variant == _variantFilter);
            }

            if (_scopeFilter is not null)
            {
                filtered = filtered.Where(l => l.Scope == _scopeFilter);
            }

            return filtered;
        }
    }

    private void OnSearchChanged(string? value) => _searchString = value;

    private void OnColourChanged(AppColour? colour) => _colourFilter = colour;

    private void OnVariantChanged(AppVariant? variant) => _variantFilter = variant;

    private void OnScopeChanged(LabelScope? scope) => _scopeFilter = scope;

    private void ClearFilters()
    {
        _searchString = null;
        _colourFilter = null;
        _variantFilter = null;
        _scopeFilter = null;
    }

    protected override IQuery<Result<LabelDto[]>> CreateQuery()
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
            ContractIds = []
        };

        var parameters = new DialogParameters<AddLabelDialog>()
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false};

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
            NewAppIcon = context.AppIcon,
            NewContractIds = context.ContractIds.ToList()
        };

        var parameters = new DialogParameters<EditLabelDialog>()
        {
            { x => x.Model, command },
            { x => x.CurrentUser, CurrentUser }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false};
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

        var label = context.ContractCount switch
        {
            1 => $"Are you sure you want to delete the {context.Name} label? It is assigned to 1 contract.",
            _ => $"Are you sure you want to delete the {context.Name} label? It is assigned to {context.ContractCount} contracts.",
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
