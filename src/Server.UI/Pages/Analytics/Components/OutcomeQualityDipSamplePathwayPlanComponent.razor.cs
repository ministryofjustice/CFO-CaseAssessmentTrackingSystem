using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.Features.PerformanceManagement.Queries;
using Cfo.Cats.Server.UI.Pages.Activities;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class OutcomeQualityDipSamplePathwayPlanComponent
{
    private bool _isLoading = true;

    [Parameter, EditorRequired] public string ParticipantId { get; set; } = null!;

    private ParticipantDipSamplePathwayPlanDto? PathwayPlan { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var mediator = GetNewMediator();
            var query = new GetOutcomeQualityDipSamplePathwayPlan.Query()
            {
                ParticipantId = ParticipantId
            };

            var result = await mediator.Send(query);
            if (IsDisposed == false)
            {
                if (result is { Succeeded: true, Data: not null })
                {
                    PathwayPlan = result;
                }
            }
        }
        finally
        {
            _isLoading = false;
        }
    }

    private string? GetIcon(ParticipantDipSampleObjectiveTaskDto task) =>
        task switch
        {
            { Completed: null } => Icons.Material.Filled.NotStarted,
            { Completed: not null, Status: "Done" } => Icons.Material.Filled.CheckCircle,
            _ => Icons.Material.Filled.HighlightOff
        };

    private Color GetColour(ParticipantDipSampleObjectiveTaskDto task) =>
        task switch
        {
            { Completed: null } => Color.Info,
            { Completed: not null, Status: "Done" } => Color.Success,
            _ => Color.Error
        };

    private string? GetIcon(ParticipantDipSampleObjectiveDto obj) =>
        obj switch
        {
            { Completed: null } => Icons.Material.Filled.Sync,
            { Completed: not null, Status: "Done" } => Icons.Material.Filled.CheckCircle,
            _ => Icons.Material.Filled.HighlightOff
        };

    private Color GetColour(ParticipantDipSampleObjectiveDto obj) =>
        obj switch
        {
            { Completed: null } => Color.Info,
            { Completed: not null, Status: "Done" } => Color.Success,
            _ => Color.Error
        };
    private async Task DisplayActivity(ParticipantDipSampleActivityDto dto)
    {
        var parameters = new DialogParameters<ActivitiyDipDialog>()
        {
            { x => x.Model, dto }
        };

        var options = new DialogOptions()
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.ExtraExtraLarge,
            FullScreen = false,
            FullWidth = true,
            CloseButton = true,
        };

        var dialog = await DialogService.ShowAsync<ActivitiyDipDialog>("Activity Details", parameters, options);

    }
}