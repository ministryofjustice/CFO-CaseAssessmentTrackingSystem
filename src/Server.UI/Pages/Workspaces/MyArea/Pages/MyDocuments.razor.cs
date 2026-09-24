using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.Features.Documents.Queries;
using Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Pages;

public partial class MyDocuments
{
    private const string FiltersSeparator = ". Filters applied:";

    private string searchString = "";
    private readonly HashSet<Guid> _expandedIds = [];

    private bool FilterFunc(GeneratedDocumentDto doc)
    {
        if (string.IsNullOrWhiteSpace(searchString)) { return true; }
        return doc.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
            || (doc.Title?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (doc.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static (string Summary, string[] Filters) SplitDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return (string.Empty, []);
        }

        var separatorIndex = description.IndexOf(FiltersSeparator, StringComparison.Ordinal);

        if (separatorIndex < 0)
        {
            return (description, []);
        }

        var summary = description[..separatorIndex];
        var filters = description[(separatorIndex + FiltersSeparator.Length)..]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return (summary, filters);
    }

    private void ToggleExpanded(Guid documentId)
    {
        if (!_expandedIds.Add(documentId))
        {
            _expandedIds.Remove(documentId);
        }
    }

    protected override IQuery<Result<GeneratedDocumentDto[]>> CreateQuery() => 
        new GetMyDocumentsQuery.Query();

    private async Task Download(GeneratedDocumentDto document)
    {
        var parameters = new DialogParameters<OnExportConfirmationDialog>()
        {
            { x => x.DocumentId, document.Id }
        };

        var dialog = await DialogService.ShowAsync<OnExportConfirmationDialog>(document.Title, parameters, new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            CloseButton = true
        });

        await dialog.Result;
    }

}
