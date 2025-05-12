using Cfo.Cats.Application.Features.Delius.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.DMS.Delius;

public partial class MainOffenceDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DisposalDto[] Disposals { get; set; } = [];
    private void Ok() => MudDialog.Cancel();

    private MudDataGrid<DisposalDto> _dataGrid = null!;
}