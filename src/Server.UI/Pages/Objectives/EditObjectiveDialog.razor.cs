using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class EditObjectiveDialog
{
    [Inject] private IInitiativeService InitiativeService { get; set; } = null!;
    [Inject] private IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private MudForm? _form;
    private bool _saving;
    private bool _canUnlinkInitiative;
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
    /// True when one or more activities have already been recorded against this objective. Regular users
    /// cannot unlink an initiative from an objective with activities recorded against it; only CMPSM+ can
    /// bypass this restriction.
    /// </summary>
    [Parameter] public bool HasActivities { get; set; }

    /// <summary>
    /// Whether the currently linked initiative belongs to the same tenant/region as the case. Only
    /// unlinking an initiative from a different tenant/region requires CMPSM+ authorization and a
    /// justification; removing one within the case's own tenant/region behaves exactly as before.
    /// </summary>
    [Parameter] public bool LinkedInitiativeInCaseTenant { get; set; } = true;

    private bool RequiresElevatedUnlink => IsChangingLinkedInitiative && LinkedInitiativeInCaseTenant is false;

    /// <summary>
    /// True when the objective currently has a linked initiative and the user's selection would change
    /// or remove that link — covers both clearing the field entirely and picking a different initiative
    /// to replace it with, since replacing effectively removes the previous link too.
    /// </summary>
    private bool IsChangingLinkedInitiative => HasLinkedInitiative && Model.InitiativeId != _originalInitiativeId;

    /// <summary>
    /// True when a regular (non-CMPSM+) user is attempting to remove an initiative that is within the
    /// case's own tenant, but activities have already been recorded against the objective — this is
    /// blocked server-side (see NotHaveActivitiesWhenChangingInitiative), so it's surfaced here too rather
    /// than only failing after a round-trip.
    /// </summary>
    private bool BlockedByActivitiesForRegularUser =>
        IsChangingLinkedInitiative && LinkedInitiativeInCaseTenant && HasActivities && _canUnlinkInitiative is false;

    private bool SaveDisabled =>
        IsChangingLinkedInitiative
        && ((RequiresElevatedUnlink && _canUnlinkInitiative is false) || BlockedByActivitiesForRegularUser);

    protected override async Task OnInitializedAsync()
    {
        _originalInitiativeId = Model.InitiativeId;

        if (Model.InitiativeId.HasValue)
        {
            _selectedInitiative = InitiativeService.DataSource.FirstOrDefault(i => i.Id == Model.InitiativeId.Value);
        }

        var authState = await AuthState;
        _canUnlinkInitiative = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.ManageInitiatives)).Succeeded;
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