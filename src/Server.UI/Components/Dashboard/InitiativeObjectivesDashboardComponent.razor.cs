using ApexCharts;
using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Initiatives.DTOs;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class InitiativeObjectivesDashboardComponent
{
    [Parameter]
    public string? UserId { get; set; }
    [Parameter]
    public string? TenantId { get; set; }

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    [Inject]
    private IInitiativeService InitiativeService { get; set; } = null!;

    private string _initiativeFilter = string.Empty;
    private string _statusFilter = string.Empty;
    private bool ShowCompletedOnly { get; set; } = false;

    private IReadOnlyList<InitiativeDto> _initiatives = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _initiatives = InitiativeService.GetActiveInitiatives(CurrentUser.TenantId!).ToList();
    }

    protected override IRequest<Result<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto[]>> CreateQuery()
     => new GetInitiativeObjectivesDashboard.Query
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId
     };

    private ApexCharts.ApexChartOptions<InitiativeChartPoint> Options => new()
    {
        Chart = new ApexCharts.Chart
        {
            Stacked = true
        },
        PlotOptions = new ApexCharts.PlotOptions
        {
            Bar = new ApexCharts.PlotOptionsBar
            {
                Horizontal = false,
                DataLabels = new ApexCharts.PlotOptionsBarDataLabels
                {
                    Total = new ApexCharts.BarTotalDataLabels
                    {
                        Enabled = true,
                        Style = new ApexCharts.BarDataLabelsStyle
                        {
                            FontWeight = "800",
                            Color = IsDarkMode ? "#FFFFFF" : "#000000",
                        }
                    }
                },
            },
        },
        Yaxis = new List<YAxis>
        {
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        },
        Theme = new ApexCharts.Theme
        {
            Mode = IsDarkMode ? ApexCharts.Mode.Dark : ApexCharts.Mode.Light
        },
        Colors = new List<string> { "#1976d2", "#5cb85c" }
    };

    private IEnumerable<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto> FilteredRows =>
        Data is null
            ? Array.Empty<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto>()
            : Data
                .Where(r => string.IsNullOrEmpty(_initiativeFilter) || r.InitiativeCode == _initiativeFilter)
                .Where(r => _statusFilter == "active" ? !r.IsObjectiveCompleted
                           : _statusFilter == "completed" ? r.IsObjectiveCompleted
                           : true)
                .Where(r => !ShowCompletedOnly || r.IsObjectiveCompleted);

    public record InitiativeChartPoint(string InitiativeCode, int ActiveCount, int CompletedCount);

    private int TotalActive =>
        ShowCompletedOnly && Data is not null
            ? 0
            : Data?.Count(r => !r.IsObjectiveCompleted) ?? 0;

    private int TotalCompleted => Data?.Count(r => r.IsObjectiveCompleted) ?? 0;

    private InitiativeChartPoint[] ChartData =>
        Data?
            .GroupBy(r => r.InitiativeCode)
            .Select(g => new InitiativeChartPoint(
                g.Key,
                g.Count(r => !r.IsObjectiveCompleted),
                g.Count(r => r.IsObjectiveCompleted)))
            .OrderBy(p => p.InitiativeCode)
            .ToArray()
        ?? Array.Empty<InitiativeChartPoint>();

    private InitiativeChartPoint[] FilteredChartData =>
        ShowCompletedOnly && Data is not null
            ? ChartData.Select(x => new InitiativeChartPoint(
                x.InitiativeCode,
                x.CompletedCount,
                x.CompletedCount))
              .Where(x => x.CompletedCount > 0)
              .ToArray()
            : ChartData ?? Array.Empty<InitiativeChartPoint>();
}
