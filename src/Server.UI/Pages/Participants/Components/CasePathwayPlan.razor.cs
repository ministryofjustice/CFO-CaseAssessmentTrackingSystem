using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Objectives;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CasePathwayPlan
{
    private bool _loading;
    private bool _hideCompletedObjectives;
    private bool _hideCompletedTasks;

    private string _selector = "Created";
    private Dictionary<string, Func<ObjectiveDto, dynamic>> _selectors = new()
    {
        { "Created", (objective) => objective.Created },
        { "Title", (objective) => objective.Description },
        { "Outstanding", (objective) => objective.Tasks.Where(task => task.IsCompleted is false).Count() },
    };

    private SortDirection _sortDirection = SortDirection.Ascending;

    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }

    [Parameter, EditorRequired]
    public bool ParticipantIsActive { get; set; }

    public PathwayPlanDto? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await OnRefresh(firstRender: true);
        await base.OnInitializedAsync();
    }

    private async Task OnRefresh(bool firstRender = false)
    {
        _loading = true;

        try
        {
            Model = await GetNewMediator().Send(new GetPathwayPlanByParticipantId.Query()
            {
                ParticipantId = ParticipantId
                           });

            if (firstRender is false)
            {
                await OnUpdate.InvokeAsync(); // Bubble update, refreshing participant information                
            }
        }
        finally
        {
            _loading = false;
        }
    }

    public async Task AddObjective()
    {
        var command = new AddObjective.Command()
        {
            PathwayPlanId = Model!.Id
        };

        var parameters = new DialogParameters<AddObjectiveDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true, BackdropClick = false };
        var dialog = await DialogService.ShowAsync<AddObjectiveDialog>("Add thematic objective", parameters, options);

        if (await dialog.Result is { Canceled: false })
        {
            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ThematicObjectiveSuccessfullyAdded, Severity.Info);
                await OnRefresh();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }

    private async Task ReviewPathwayPlan()
    {
        var command = new ReviewPathwayPlan.Command
        {
            PathwayPlanId = Model!.Id,
            ParticipantId = ParticipantId,
            ReviewDate = DateTime.UtcNow,
            ReviewReason = PathwayPlanReviewReason.Default
        };

        var parameters = new DialogParameters<ReviewPathwayPlanDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        
        var dialog = await DialogService.ShowAsync<ReviewPathwayPlanDialog>("Review Pathway Plan", parameters, options);
      
        if (await dialog.Result is { Canceled: false })
        {
            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.PathwayPlanSuccessfullyReviewed, Severity.Info);
                await OnRefresh();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }
}