using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Models;
using Cfo.Cats.Server.UI.Pages.Objectives;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class ViewPathwayPlan
{
    private bool _loading;
    private bool _hideCompletedObjectives;
    private bool _hideCompletedTasks;

    private string _selector = "Created";
    private readonly Dictionary<string, Func<ObjectiveDto, dynamic>> _selectors = new()
    {
        { "Created", (objective) => objective.Created },
        { "Title", (objective) => objective.Description },
        { "Outstanding", (objective) => objective.Tasks.Where(task => task.IsCompleted is false).Count() },
    };

    private SortDirection _sortDirection = SortDirection.Ascending;

    [CascadingParameter(Name = "ParticipantDetails")]
    public ParticipantCascadingDetails? ParticipantDetails { get; set; }
    
    public PathwayPlanDto? Model { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (ParticipantDetails is null)
        {
            return;
        }

        await OnRefresh(firstRender: true);
    }
    
    private async Task OnRefresh(bool firstRender = false)
    {
        if (ParticipantDetails is null)
        {
            return;
        }

        _loading = true;

        try
        {
            Model = await GetNewMediator().Send(new GetPathwayPlanByParticipantId.Query()
            {
                ParticipantId = ParticipantDetails.Id
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

    private async Task AddObjective()
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
        var dialogTitle = $"Add thematic objective for {ParticipantDetails?.FullName} Ref: {ParticipantDetails?.Id}";
        var dialog = await DialogService.ShowAsync<AddObjectiveDialog>(dialogTitle, parameters, options);

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
            ParticipantId = ParticipantDetails!.Id,
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