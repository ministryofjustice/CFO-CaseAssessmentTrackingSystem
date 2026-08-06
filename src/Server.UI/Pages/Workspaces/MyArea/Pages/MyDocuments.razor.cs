using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.Features.Documents.Queries;
using Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Pages;

public partial class MyDocuments
{
    private string searchString = "";

    private bool FilterFunc(GeneratedDocumentDto doc)
    {
        if (string.IsNullOrWhiteSpace(searchString)) { return true; }
        return doc.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
            || (doc.Title?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (doc.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false);
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
