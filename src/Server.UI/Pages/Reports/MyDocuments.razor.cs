using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.Features.Documents.Queries;

namespace Cfo.Cats.Server.UI.Pages.Reports;

public partial class MyDocuments
{
    GeneratedDocumentDto[] documents = [];

    bool loading = false;
    private string searchString = "";

    protected override async Task OnInitializedAsync()
    {
        documents = await GetNewMediator().Send(new GetMyDocumentsQuery.Query());
    }

    async Task Download(GeneratedDocumentDto document)
    {
        await Task.CompletedTask;
    }

}
