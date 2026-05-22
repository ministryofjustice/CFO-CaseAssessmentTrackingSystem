using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseNotes
{
    private ParticipantNoteDto[]? _notes;
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

    private IEnumerable<ParticipantNoteDto> OrderedNotes =>
        _notes?
            .OrderByDescending(x => x.Created)!;

    private IEnumerable<ParticipantNoteDto> PagedNotes =>
        OrderedNotes
            .Skip((_pageNumber - 1) * PageSize)
            .Take(PageSize);

    private int TotalNotes => _notes?.Length ?? 0;

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalNotes / (double)PageSize));

    private async Task OnRefresh()
    {
        _notes = await GetNewMediator().Send(new GetParticipantNotes.Query()
        {
            ParticipantId = ParticipantId
        });

        if (_pageNumber > TotalPages)
        {
            _pageNumber = TotalPages;
        }
    }

    private Task OnPaginationChanged(int page)
    {
        _pageNumber = page;
        return Task.CompletedTask;
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
