using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class ActivityDipDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired] public ParticipantDipSampleActivityDto Model { get; set; } = null!;

    private void Ok() => MudDialog.Cancel();

    private Guid? DocumentId => Model switch
    {
        ParticipantDipSampleEmploymentActivityDto emp => emp.DocumentId,
        ParticipantDipSampleEducationAndTrainingActivityDto ete => ete.DocumentId,
        ParticipantDipSampleIswActivityDto isw => isw.DocumentId,
        _ => null
    };

    private bool DisableDocumentTab()
        => DocumentId is null;
}