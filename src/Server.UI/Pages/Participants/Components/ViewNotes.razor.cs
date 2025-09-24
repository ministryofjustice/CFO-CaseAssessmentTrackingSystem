using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class ViewNotes
{
    protected override IRequest<Result<ParticipantNoteDto[]>> CreateQuery()
        => new GetParticipantNotes.Query()
        {
            ParticipantId = ParticipantId
        };
}