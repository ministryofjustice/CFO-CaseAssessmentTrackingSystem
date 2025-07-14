using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.Features.Documents.Queries;
using Cfo.Cats.Server.UI.Pages.Analytics.Components;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class MyDocuments
{
    GeneratedDocumentDto[] documents = [];

    bool loading = false;
    private string searchString = "";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            loading = true;
            documents = await GetNewMediator().Send(new GetMyDocumentsQuery.Query());
        }
        finally { loading = false; }
    }

    async Task Download(GeneratedDocumentDto document)
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
