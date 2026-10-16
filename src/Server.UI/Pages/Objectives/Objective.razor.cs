using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Activities;
using Cfo.Cats.Server.UI.Pages.Objectives.Tasks;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class Objective
{
    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }

    [Parameter, EditorRequired]
    public bool ParticipantIsActive { get; set; }

    [Parameter, EditorRequired]
    public required string ParticipantName { get; set; }
    
    [Parameter, EditorRequired]
    public required ObjectiveDto Model { get; set; }

    [Parameter, EditorRequired]
    public required bool HideCompletedTasks { get; set; }

    [Parameter]
    public bool Expanded { get; set; } = true;

    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = null!;

    private async Task Complete()
    {
        var command = new CompleteObjective.Command()
        {
            PathwayPlanId = Model.PathwayPlanId,
            ObjectiveId = Model.Id
        };

        var parameters = new DialogParameters<CompleteObjectiveDialog>()
        {
            { x => x.Model, command },
            { x => x.HasLinkedInitiative, Model.LinkedInitiative is not null }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var dialogTitle = "Complete objective for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<CompleteObjectiveDialog>(dialogTitle, parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            var result = await GetNewMediator().Send(command);
                                
            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ObjectiveSuccessfullyCompleted, Severity.Info);
                await OnChange.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }

    private async Task AddTask()
    {
        var command = new AddTask.Command()
        {
            PathwayPlanId = Model.PathwayPlanId,
            ObjectiveId = Model.Id
        };

        var parameters = new DialogParameters<AddTaskDialog>()
        {
            { x => x.Model, command }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Add task to objective for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<AddTaskDialog>(dialogTitle, parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.TaskSuccessfullyAddedToObjective, Severity.Info);
                await OnChange.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }

    private async Task Expand()
    {
        var options = SetDialogOptions();
        await DialogService.ShowAsync<ExpandObjectiveDialog>
        (
        Model.Description,
        new DialogParameters<ExpandObjectiveDialog>()
        {
            { x => x.Model, Model }
        },
        options
        );
    }

    private async Task Edit()
    {
        var authState = await AuthState;
        var canOverrideInitiativeRestrictions = (await AuthService.AuthorizeAsync(authState.User, SecurityPolicies.ManageInitiatives)).Succeeded;

        var command = new EditObjective.Command()
        {
            PathwayPlanId = Model.PathwayPlanId,
            ObjectiveId = Model.Id,
            Description = Model.Description,
            InitiativeId = Model.LinkedInitiative?.Id,
            InitiativeStartDate = Model.LinkedInitiative?.InitiativeStartDate?.ToDateTime(TimeOnly.MinValue),
            CanOverrideInitiativeRestrictions = canOverrideInitiativeRestrictions
        };

        var parameters = new DialogParameters<EditObjectiveDialog>()
        {
            { x => x.Model, command },
            { x => x.HasLinkedInitiative, Model.LinkedInitiative is not null }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Edit objective for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<EditObjectiveDialog>(dialogTitle, parameters, options);

        var state = await dialog.Result;

        // The dialog sends the command itself and only closes on success.
        if (state!.Canceled is false)
        {
            Snackbar.Add(ConstantString.ObjectiveSuccessfullyEdited, Severity.Info);
            await OnChange.InvokeAsync();
        }
    }

    private async Task ShowActivityHistory()
    {
        var parameters = new DialogParameters<ActivitiesDialog>()
        {
            {
                x => x.Model, new ActivitiesWithPagination.Query()
                {
                    ParticipantId = ParticipantId,
                    ObjectiveId = Model.Id
                }
            },
            { x => x.Objective, Model }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Activity/ETE history for objective " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<ActivitiesDialog>(dialogTitle, parameters, options);

        if (await dialog.Result is not { Canceled: true })
        {
            await OnChange.InvokeAsync();
        }
    }

    private DialogOptions SetDialogOptions() => new DialogOptions
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        BackdropClick = false
    };
}