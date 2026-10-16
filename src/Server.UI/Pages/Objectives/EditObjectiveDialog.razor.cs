using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class EditObjectiveDialog
{
    [Inject] private IInitiativeService InitiativeService { get; set; } = null!;
    [Inject] private IServiceScopeFactory ScopeFactory { get; set; } = null!;

    /// <summary>
    /// Field-level validation. IncludeProperties without IncludeRuleSets runs every rule set for the
    /// property — including the database-backed Mediator rules (e.g. justification required) — so users
    /// get inline feedback. Each validation runs in its own DI scope (and therefore its own UnitOfWork /
    /// DbContext) because MudForm can fire validations concurrently (e.g. on every keystroke of an
    /// Immediate field), which would otherwise overlap on the circuit's shared, scoped DbContext.
    /// Rules not bound to a form field (e.g. on InitiativeId) are still enforced when Submit sends the
    /// command, and the dialog stays open if they fail.
    /// </summary>
    private Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        await using var scope = ScopeFactory.CreateAsyncScope();
        var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
        return await validationService.ValidatePropertyAsync((EditObjective.Command)model, propertyName.Split('.').Last());
    };

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
                return;
            }

            await _form.ValidateAsync();

            if (_form.IsValid is false)
            {
                return;
            }

            // Send from here (rather than the caller) so the Mediator rule set is evaluated before the
            // dialog closes — if it fails, the error is shown and the user's edits are kept.
            var result = await GetNewMediator().Send(Model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }
}
