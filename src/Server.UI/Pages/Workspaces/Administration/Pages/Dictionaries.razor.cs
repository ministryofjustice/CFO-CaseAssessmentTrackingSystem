using Cfo.Cats.Application.Features.KeyValues.Caching;
using Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;
using Cfo.Cats.Application.Features.KeyValues.Commands.Delete;
using Cfo.Cats.Application.Features.KeyValues.Commands.Export;
using Cfo.Cats.Application.Features.KeyValues.DTOs;
using Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

public partial class Dictionaries
{
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    private MudDataGrid<KeyValueDto> _table = null!;
    public string Title { get; set; } = "Picklist";
    private HashSet<KeyValueDto> _selectedItems = [];
    private KeyValueDto SelectedItem { get; set; } = new();
    private string _searchString = string.Empty;
    private Picklist? _searchPicklist;
    private int _defaultPageSize = 15;
    private KeyValuesWithPaginationQuery Query { get; set; } = new();
    private bool _canCreate;
    private bool _canSearch;
    private bool _canDelete;
    private bool _canExport;
    private bool _loading;
    private bool _downloading;

    protected override async Task OnInitializedAsync()
    {
        Title = L[SelectedItem.GetClassDescription()];
        var state = await AuthState;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemFunctionsWrite)).Succeeded;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemFunctionsRead)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SystemFunctionsWrite)).Succeeded;
        _canExport = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SeniorInternal)).Succeeded;
    }

    private async Task<GridData<KeyValueDto>> ServerReload(GridState<KeyValueDto> state,
        CancellationToken cancellationToken)
    {
        try
        {
            _loading = true;
            var request = new KeyValuesWithPaginationQuery
            {
                Keyword = _searchString,
                Picklist = _searchPicklist,
                OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "Id",
                SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true
                    ? nameof(SortDirection.Descending)
                    : nameof(SortDirection.Ascending),
                PageNumber = state.Page + 1,
                PageSize = state.PageSize
            };
            var result = await GetNewMediator().Send(request, cancellationToken: cancellationToken);
            return new GridData<KeyValueDto>
            {
                TotalItems = result.TotalItems,
                Items = result.Items
            };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearch(string text)
    {
        _searchString = text;
        await _table.ReloadServerData();
    }

    private async Task OnSearch(Picklist? val)
    {
        _searchPicklist = val;
        await _table.ReloadServerData();
    }

    private async Task OnRefresh()
    {
        KeyValueCacheKey.Refresh();
        _searchString = string.Empty;
        await _table.ReloadServerData();
    }

    private async Task<DataGridEditFormAction> CommittedItemChanges(KeyValueDto item)
    {
        await InvokeAsync(async () =>
        {
            var command = Mapper.Map<AddEditKeyValueCommand>(item);
            var result = await GetNewMediator().Send(command);
            if (!result.Succeeded)
            {
                Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
            }

            StateHasChanged();
        });
        return DataGridEditFormAction.Close;
    }

    private async Task DeleteItem(KeyValueDto item)
    {
        var deleteContent = ConstantString.DeleteConfirmation;
        var command = new DeleteKeyValueCommand([
            item.Id
        ]);
        var parameters = new DialogParameters<DeleteConfirmation>
        {
            {
                x => x.Command, command
            },
            {
                x => x.ContentText, string.Format(deleteContent, item.Name)
            }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true
        };
        var dialog =
            await DialogService.ShowAsync<DeleteConfirmation>(ConstantString.DeleteConfirmationTitle, parameters,
                options);
        var state = await dialog.Result;
        if (!state!.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task OnCreate()
    {
        var command = new AddEditKeyValueCommand
        {
            Name = SelectedItem.Name,
            Description = SelectedItem.Description
        };
        var parameters = new DialogParameters<CreatePicklistDialog>
        {
            {
                x => x.Model, command
            }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
        var dialog = await DialogService.ShowAsync<CreatePicklistDialog>
            (L["Create a new picklist"], parameters, options);
        var state = await dialog.Result;
        if (!state!.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await GetNewMediator().Send(new ExportKeyValues.Command()
            {
                Query = Query
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}
