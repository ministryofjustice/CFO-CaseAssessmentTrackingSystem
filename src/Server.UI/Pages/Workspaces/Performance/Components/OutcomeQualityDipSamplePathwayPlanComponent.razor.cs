using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.Features.PerformanceManagement.Queries;
using Cfo.Cats.Server.UI.Pages.Activities;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components;

public partial class OutcomeQualityDipSamplePathwayPlanComponent
{
    private bool _isLoading = true;

    private MudExpansionPanels? _objectivePanels;
    private readonly Dictionary<int, MudExpansionPanels> _taskPanelsByObjective = new();

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

    public async Task ExpandAll()
    {
        if (_objectivePanels is not null)
        {
            await _objectivePanels.ExpandAllAsync();
        }

        foreach (var panels in _taskPanelsByObjective.Values)
        {
            await panels.ExpandAllAsync();
        }
    }

    public async Task CollapseAll()
    {
        if (_objectivePanels is not null)
        {
            await _objectivePanels.CollapseAllAsync();
        }

        foreach (var panels in _taskPanelsByObjective.Values)
        {
            await panels.CollapseAllAsync();
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
        var parameters = new DialogParameters<ActivityDipDialog>()
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

        var dialog = await DialogService.ShowAsync<ActivityDipDialog>("Activity Details", parameters, options);

    }
}