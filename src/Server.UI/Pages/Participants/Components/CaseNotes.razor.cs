using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components
{
    public partial class CaseNotes
    {
        ParticipantNoteDto[]? _notes = null;

        [Parameter, EditorRequired]
        public string ParticipantId { get; set; } = default!;

        [Parameter, EditorRequired]
        public bool ParticipantIsActive { get; set; } = default!;

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

        private async Task OnRefresh()
        {
            _notes = await GetNewMediator().Send(new GetParticipantNotes.Query()
            {
                ParticipantId = ParticipantId
            });
        }

        public async Task OnAddNote()
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
                await OnRefresh();
            }
        }

        public async Task OnExpandNote(string message)
        {
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
            await DialogService.ShowMessageBox("Note", message, options: options);
        }
    }
}