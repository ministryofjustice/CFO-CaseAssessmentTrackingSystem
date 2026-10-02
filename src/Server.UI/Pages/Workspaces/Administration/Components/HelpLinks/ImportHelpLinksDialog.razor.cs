using Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Infrastructure.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.HelpLinks;

public partial class ImportHelpLinksDialog
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private bool _busy;
    private string? _errorMessage;
    private HelpLinkImportPreviewItemDto[]? _previewItems;
    private readonly Dictionary<string, bool> _useImported = new();

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    private void Cancel() => MudDialog.Cancel();

    private static string ConflictKey(HelpLinkImportPreviewItemDto item) => $"{item.PageKey}|{item.TabName}";

    private void SetAllConflicts(bool useImported)
    {
        foreach (var key in _useImported.Keys.ToArray())
        {
            _useImported[key] = useImported;
        }
    }

    private async Task OnFileSelected(IBrowserFile file)
    {
        _errorMessage = null;
        _busy = true;

        try
        {
            await using var stream = file.OpenReadStream(MaxFileSizeBytes);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            var result = await Service.Send(new PreviewHelpLinksImportCommand(memoryStream.ToArray()));

            if (result.Succeeded == false || result.Data is null)
            {
                _errorMessage = result.ErrorMessage;
                return;
            }

            _previewItems = result.Data;
            _useImported.Clear();
            foreach (var item in _previewItems.Where(x => x.IsConflict))
            {
                // Default every conflict to keeping the existing entry
                _useImported[ConflictKey(item)] = false;
            }
        }
        catch (Exception)
        {
            _errorMessage = "Could not read the uploaded file. Please check it is a valid HelpLinks export and try again.";
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task Confirm()
    {
        if (_previewItems is null)
        {
            return;
        }

        _busy = true;

        try
        {
            var decisions = new List<HelpLinkImportDecisionDto>();

            foreach (var item in _previewItems)
            {
                var include = item.Status switch
                {
                    HelpLinkImportMatchStatus.New => true,
                    HelpLinkImportMatchStatus.Conflict => _useImported.GetValueOrDefault(ConflictKey(item)),
                    _ => false // Unchanged - nothing to do
                };

                if (include == false)
                {
                    continue;
                }

                decisions.Add(new HelpLinkImportDecisionDto
                {
                    Title = item.Incoming.Title,
                    Description = item.Incoming.Description,
                    Urls = item.Incoming.Urls,
                    PageKey = item.PageKey,
                    TabName = item.TabName,
                    ExistingHelpLinkId = item.ExistingHelpLinkId
                });
            }

            if (decisions.Count == 0)
            {
                Snackbar.Add("Nothing was imported.", Severity.Info);
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }

            var result = await Service.Send(new ApplyHelpLinksImportCommand { Decisions = decisions });

            if (result.Succeeded == false || result.Data is null)
            {
                _errorMessage = result.ErrorMessage;
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));

            var message = $"{result.Data.Created} created, {result.Data.Updated} updated.";
            if (result.Data.Skipped > 0)
            {
                message += $" {result.Data.Skipped} skipped (already existed).";
            }

            Snackbar.Add(message, Severity.Success);
        }
        finally
        {
            _busy = false;
        }
    }
}
