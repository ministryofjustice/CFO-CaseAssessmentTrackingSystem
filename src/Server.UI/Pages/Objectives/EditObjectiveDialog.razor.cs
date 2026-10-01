using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class EditObjectiveDialog
{
    [Inject] private IInitiativeService InitiativeService { get; set; } = null!;

    private MudForm? _form;
    private bool _saving;
    private InitiativeDto? _selectedInitiative;
    private Guid? _originalInitiativeId;

    private InitiativeDto? SelectedInitiative
    {
        get => _selectedInitiative;
        set
        {
            _selectedInitiative = value;
            Model.InitiativeId = value?.Id;
            if (value is null)
            {
                Model.InitiativeStartDate = null;
            }

            // Only clear the justification when there's no initiative being changed away from (i.e. a
            // fresh link with nothing previously linked) or the user has picked the original initiative
            // back again (no effective change) — otherwise preserve it, since it may still be required
            // for a cross-tenant swap to a different initiative, not just an outright removal.
            if (HasLinkedInitiative is false || value?.Id == _originalInitiativeId)
            {
                Model.Justification = null;
            }
        }
    }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired] public required EditObjective.Command Model { get; set; }
    [Parameter, EditorRequired] public bool HasLinkedInitiative { get; set; }

    /// <summary>
    /// True when the objective currently has a linked initiative and the user's selection would change
    /// or remove that link — covers both clearing the field entirely and picking a different initiative
    /// to replace it with, since replacing effectively removes the previous link too. Whether this
    /// change is actually permitted (authorization, justification, recorded activities, etc.) is an
    /// unlinking concern enforced server-side by EditObjective's command validation.
    /// </summary>
    private bool IsChangingLinkedInitiative => HasLinkedInitiative && Model.InitiativeId != _originalInitiativeId;

    protected override void OnInitialized()
    {
        _originalInitiativeId = Model.InitiativeId;

        if (Model.InitiativeId.HasValue)
        {
            _selectedInitiative = InitiativeService.DataSource.FirstOrDefault(i => i.Id == Model.InitiativeId.Value);
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        try
        {
            _saving = true;

            if (_form is null)
            {
                _saving = false;
                return;
            }

            await _form.ValidateAsync();

            if (_form.IsValid)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        finally
        {
            _saving = false;
        }
    }
}
