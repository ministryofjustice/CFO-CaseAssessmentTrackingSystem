using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class ParticipantCardView
{
    [Parameter, EditorRequired]
    public ParticipantPaginationDto[] Items { get; set; } = null!;

    [Parameter, EditorRequired]
    public ParticipantsWithPagination.Query Query { get; set; } = null!;

    [Parameter]
    public bool CanReassign { get; set; }

    [Parameter, EditorRequired]
    public HashSet<string> SelectedParticipantIds { get; set; } = null!;

    [Parameter]
    public EventCallback<ParticipantPaginationDto> OnViewParticipant { get; set; }

    [Parameter]
    public EventCallback<(string ParticipantId, bool IsSelected)> OnParticipantSelectionChanged { get; set; }

    private bool IsParticipantSelected(string participantId) => SelectedParticipantIds.Contains(participantId);
}
