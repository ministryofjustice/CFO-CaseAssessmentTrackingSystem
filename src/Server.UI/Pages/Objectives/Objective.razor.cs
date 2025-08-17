using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Activities;
using Cfo.Cats.Server.UI.Pages.Objectives.Tasks;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class Objective
{
    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }

    [Parameter, EditorRequired]
    public bool ParticipantIsActive { get; set; } = default!;

    [Parameter, EditorRequired]
    public required ObjectiveDto Model { get; set; }

    [Parameter, EditorRequired]
    public required bool HideCompletedTasks { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    public async Task Complete()
    {
        var command = new CompleteObjective.Command()
        {
            PathwayPlanId = Model.PathwayPlanId,
            ObjectiveId = Model.Id
        };

        var parameters = new DialogParameters<CompleteObjectiveDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
        var dialog = await DialogService.ShowAsync<CompleteObjectiveDialog>("Complete objective", parameters, options);

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

    public async Task AddTask()
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
        var dialog = await DialogService.ShowAsync<AddTaskDialog>("Add task to objective", parameters, options);

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

    public async Task Expand()
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

    public async Task Rename()
    {
        var command = new EditObjective.Command()
        {
            PathwayPlanId = Model.PathwayPlanId,
            ObjectiveId = Model.Id,
            Description = Model.Description
        };

        var parameters = new DialogParameters<RenameObjectiveDialog>()
        {
            { x => x.Model, command }
        };

        var options = SetDialogOptions();
        var dialog = await DialogService.ShowAsync<RenameObjectiveDialog>("Rename objective", parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {                    
                Snackbar.Add(ConstantString.ObjectiveSuccessfullyRenamed, Severity.Info);
                await OnChange.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
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
        var dialog = await DialogService.ShowAsync<ActivitiesDialog>("Activity/ETE history for objective", parameters, options);

        if (await dialog.Result is not { Canceled: true })
        {
            await OnChange.InvokeAsync();
        }
    }

    private DialogOptions SetDialogOptions()
    {
        return new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true,
            BackdropClick = false
        };
    }
}