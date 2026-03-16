using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Activities;

namespace Cfo.Cats.Server.UI.Pages.Objectives.Tasks;

public partial class ObjectiveTask
{
    [CascadingParameter] public required ObjectiveDto Objective { get; set; }

    [Parameter, EditorRequired] public required ObjectiveTaskDto Model { get; set; }

    [Parameter, EditorRequired] public required Guid PathwayPlanId { get; set; }

    [Parameter] public EventCallback OnChange { get; set; }

    [Parameter, EditorRequired] public required string ParticipantId { get; set; }

    [Parameter, EditorRequired] public required bool ParticipantIsActive { get; set; }

    [Parameter, EditorRequired] public required string ParticipantName { get; set; }

    private async Task Complete()
    {
        var command = new CompleteTask.Command()
        {
            TaskId = Model.Id,
            ObjectiveId = Model.ObjectiveId,
            PathwayPlanId = PathwayPlanId
        };

        var parameters = new DialogParameters<CompleteTaskDialog>()
        {
            { x => x.Model, command }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Complete task for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<CompleteTaskDialog>(dialogTitle, parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            try
            {
                var result = await GetNewMediator().Send(command);

                if (result.Succeeded)
                {
                    Snackbar.Add(ConstantString.TaskSuccessfullyCompleted, Severity.Info);
                    await OnChange.InvokeAsync();
                }
                else
                {
                    Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
                }
            }
            catch (ApplicationException ae)
            {
                Snackbar.Add(ae.Message, Severity.Error);
            }
        }
    }

    private async Task Expand()
    {
        var options = SetDialogOptions();

        await DialogService.ShowAsync<ExpandTaskDialog>(
            Model.Description,
            new DialogParameters<ExpandTaskDialog>()
            {
                { x => x.Model, Model }
            },
            options: options
        );
    }

    private async Task AdjustDate()
    {
        var command = new EditTask.Command()
        {
            TaskId = Model.Id,
            ObjectiveId = Model.ObjectiveId,
            Description = Model.Description,
            Due = Model.Due,
            PathwayPlanId = PathwayPlanId
        };

        var parameters = new DialogParameters<AdjustDateTaskDialog>()
        {
            { x => x.Model, command }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Adjust date for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<AdjustDateTaskDialog>(dialogTitle, parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.DateSuccessfullyAdjusted, Severity.Info);
                await OnChange.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }

    private async Task Rename()
    {
        var command = new EditTask.Command()
        {
            TaskId = Model.Id,
            ObjectiveId = Model.ObjectiveId,
            Description = Model.Description,
            Due = Model.Due,
            PathwayPlanId = PathwayPlanId
        };

        var parameters = new DialogParameters<RenameTaskDialog>()
        {
            { x => x.Model, command }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Rename task for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<RenameTaskDialog>(dialogTitle, parameters, options);

        var state = await dialog.Result;

        if (state!.Canceled is false)
        {
            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.TaskSuccessfullyRenamed, Severity.Info);
                await OnChange.InvokeAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }

    private async Task AddActivity()
    {
        var parameters = new DialogParameters<AddActivityDialog>()
        {
            {
                x => x.Model, new AddActivity.Command(ParticipantId)
                {
                    TaskId = Model.Id
                }
            },
            { x => x.ParticipantId, ParticipantId }
        };

        var options = SetDialogOptions();
        var dialogTitle = "Add Activity/ETE for " + ParticipantName + " Ref: " + ParticipantId;
        var dialog = await DialogService.ShowAsync<AddActivityDialog>(dialogTitle, parameters, options);

        if (await dialog.Result is not { Canceled: true })
        {
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
                    TaskId = Model.Id
                }
            },
            { x => x.Task, Model },
            { x => x.Objective, Objective }
        };

        var options = SetDialogOptions();

        var dialog =
            await DialogService.ShowAsync<ActivitiesDialog>("Activity/ETE history for task", parameters, options);

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