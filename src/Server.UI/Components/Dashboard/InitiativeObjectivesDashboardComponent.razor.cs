using ApexCharts;
using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;

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
    private ITenantService TenantService { get; set; } = null!;

    private InitiativeDto? _initiativeFilter;
    private string _tenantFilter = string.Empty;
    private bool ShowActiveOnly { get; set; } = false;

    private IReadOnlyList<TenantDto> _tenants = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _tenants = TenantService.GetVisibleTenants(TenantId ?? CurrentUser.TenantId!).ToList();
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
        Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Rotate = -45,
                RotateAlways = true
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
                .Where(r => _initiativeFilter is null || r.InitiativeCode == _initiativeFilter.Code)
                .Where(r => string.IsNullOrEmpty(_tenantFilter) || r.OwnerTenantId == _tenantFilter)
                .Where(r => !ShowActiveOnly || !r.IsObjectiveCompleted);

    public record InitiativeChartPoint(string InitiativeCode, int ActiveCount, int CompletedCount);

    private int TotalActive => Data?.Count(r => !r.IsObjectiveCompleted) ?? 0;

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
        ShowActiveOnly && Data is not null
            ? ChartData.Where(x => x.ActiveCount > 0).ToArray()
            : ChartData ?? Array.Empty<InitiativeChartPoint>();
}
