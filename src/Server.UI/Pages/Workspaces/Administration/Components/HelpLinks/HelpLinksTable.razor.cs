using Cfo.Cats.Application.Features.HelpLinks.Commands.AddHelpLink;
using Cfo.Cats.Application.Features.HelpLinks.Commands.DeleteHelpLink;
using Cfo.Cats.Application.Features.HelpLinks.Commands.EditHelpLink;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.Features.HelpLinks.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;
using BlazorDownloadFile;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.HelpLinks;

public partial class HelpLinksTable
{
    [Inject]
    private IBlazorDownloadFileService BlazorDownloadFileService { get; set; } = null!;

    private string? _searchString;

    /// <summary>
    /// When supplied (e.g. via the help icon's "Manage Help Links" shortcut), a dialog is opened
    /// automatically as soon as the table loads: the Edit dialog if a help link already exists for
    /// this page/tab, otherwise the Add dialog pre-filled with these values.
    /// </summary>
    [Parameter]
    public string? InitialPageKey { get; set; }

    [Parameter]
    public string? InitialTabName { get; set; }

    private IEnumerable<HelpLinkDto> FilteredData
    {
        get
        {
            if (Data is null)
            {
                return [];
            }

            if (string.IsNullOrWhiteSpace(_searchString))
            {
                return Data;
            }

            var search = _searchString.Trim();
            return Data.Where(l =>
                l.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                || l.PageKey.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (l.TabName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }
    }

    private void OnSearchChanged(string? value) => _searchString = value;

    private async Task OnExport()
    {
        var result = await Service.Send(new ExportHelpLinks.Query());

        if (result.Succeeded == false || result.Data is null)
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
            return;
        }

        var fileName = $"help-links-{DateTime.UtcNow:yyyy-MM-dd-HHmm}.json";
        await BlazorDownloadFileService.DownloadFile(fileName, result.Data, "application/json");
    }

    private async Task OnImport()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Medium, CloseOnNavigation = false };
        var result = await DialogService.ShowAsync<ImportHelpLinksDialog>("Import Help Links", options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    protected override IQuery<Result<HelpLinkDto[]>> CreateQuery() => new GetHelpLinks.Query();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (string.IsNullOrWhiteSpace(InitialPageKey) == false)
        {
            var existing = Data?.FirstOrDefault(l =>
                string.Equals(l.PageKey, InitialPageKey, StringComparison.OrdinalIgnoreCase)
                && string.Equals(l.TabName, InitialTabName, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
            {
                await OnEdit(existing);
            }
            else
            {
                await OnAddHelpLink(InitialPageKey, InitialTabName);
            }
        }
    }

    private async Task OnAddHelpLink(string? pageKey = null, string? tabName = null)
    {
        AddHelpLinkCommand command = new()
        {
            Title = string.Empty,
            Description = string.Empty,
            Urls = [new HelpLinkUrlDto()],
            PageKey = pageKey ?? string.Empty,
            TabName = tabName
        };

        var parameters = new DialogParameters<AddHelpLinkDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false };

        var result = await DialogService.ShowAsync<AddHelpLinkDialog>("Add Help Link", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnEdit(HelpLinkDto context)
    {
        var command = new EditHelpLinkCommand()
        {
            HelpLinkId = context.Id,
            NewTitle = context.Title,
            NewDescription = context.Description,
            NewUrls = context.Urls.Select(u => new HelpLinkUrlDto { Url = u.Url, DisplayName = u.DisplayName }).ToList(),
            NewPageKey = context.PageKey,
            NewTabName = context.TabName
        };

        var parameters = new DialogParameters<EditHelpLinkDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, CloseOnNavigation = false };
        var result = await DialogService.ShowAsync<EditHelpLinkDialog>("Edit Help Link", parameters, options);
        var dialogResult = await result.Result;
        if (dialogResult!.Canceled == false)
        {
            await RefreshAsync();
        }
    }

    private async Task OnDelete(HelpLinkDto context)
    {
        var command = new DeleteHelpLinkCommand()
        {
            HelpLinkId = context.Id
        };

        var parameters = new DialogParameters<ConfirmationDialog>()
        {
            { x => x.ContentText, $"Are you sure you want to delete the \"{context.Title}\" help link?" }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true };
        var result = await DialogService.ShowAsync<ConfirmationDialog>(ConstantString.DeleteHeader, parameters, options);
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
