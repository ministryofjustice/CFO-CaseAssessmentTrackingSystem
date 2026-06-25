using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseNotes
{
    private PaginatedData<ParticipantNoteDto>? _paginatedNotes;
    private const int PageSize = 5;
    private int _pageNumber = 1;

    [Parameter, EditorRequired]
    public string ParticipantId { get; set; } = null!;

    [Parameter]
    public bool AllowAddNote { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await OnRefresh();
        }
        catch (NotFoundException)
        {
            // handle not found
        }
        finally
        {
            await base.OnInitializedAsync();
        }
    }

    private IEnumerable<ParticipantNoteDto> PagedNotes => _paginatedNotes?.Items ?? [];

    private int TotalNotes => _paginatedNotes?.TotalItems ?? 0;

    private int TotalPages => _paginatedNotes?.TotalPages ?? 0;

    private async Task OnRefresh()
    {
        var result = await GetNewMediator().Send(new GetParticipantNotes.Query()
        {
            ParticipantId = ParticipantId,
            PageNumber = _pageNumber,
            PageSize = PageSize
        });
        
        if (result.Succeeded)
        {
            _paginatedNotes = result.Data;
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
            _paginatedNotes = null;
        }
    }

    private async Task OnPaginationChanged(int page)
    {
        _pageNumber = page;
        await OnRefresh();
    }

    private async Task OnAddNote()
    {
        // Show Dialog
        var parameters = new DialogParameters<AddNoteDialog>
        {
            { x => x.Model, new AddNote.Command() { ParticipantId = ParticipantId, Message = string.Empty } }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<AddNoteDialog>
            ("Add a note", parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            _pageNumber = 1;
            await OnRefresh();
        }
    }
}
