using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.Features.Documents.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseDocuments
{
    private bool _loading = false;
    private DocumentDto[] _documents = [];
    private DocumentDto _currentDto = new() { Id = Guid.Empty };
    private string _searchString = string.Empty;

    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; } = null!;

    protected Guid SelectedDocument { get; set; } = Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;
            if (String.IsNullOrWhiteSpace(ParticipantId) == false)
            {
                _documents = await GetNewMediator().Send(new GetByParticipantId.Query()
                {
                    ParticipantId = ParticipantId
                });
            }
        }
        finally
        {
            _loading = false;
        }
    }

    public async Task OpenDocumentDialog(DocumentDto item) => await DialogService.ShowAsync<ViewDocumentDialog>(
        "Document",
        new DialogParameters<ViewDocumentDialog>()
        {
            { x => x.Model, item }
        },
        new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraExtraLarge,
            Position = DialogPosition.Center,
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true,
        });

    private Func<DocumentDto, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchString))
        {
            return true;
        }

        if (x.Title?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (x.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (x.CreatedByName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (x.TenantName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    };
}