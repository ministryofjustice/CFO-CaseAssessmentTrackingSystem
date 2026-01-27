using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

public partial class ArchivedCasesByRegionAndReasonDashboardComponent
{
    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }

    [Parameter]
    public string UserId { get; set; } = null!;

    [Parameter]
    public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    protected override IRequest<Result<GetArchivedCasesByRegionAndReason.ArchivedCasesByRegionAndReasonDto>>
        CreateQuery()
        => new GetArchivedCasesByRegionAndReason.Query
        {
            CurrentUser = CurrentUser,
            UserId = UserId,
            TenantId = TenantId,
            StartDate = DateRange?.Start
                ?? throw new InvalidOperationException("DateRange not set"),
            EndDate = DateRange?.End
                ?? throw new InvalidOperationException("DateRange not set")
        };

    private ApexChartOptions<GetArchivedCasesByRegionAndReason.ArchivedCasesChartData> Options
        => new()
        {
            Chart = new Chart
            {
                Stacked = true
            },
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    DataLabels = new PlotOptionsBarDataLabels
                    {
                        Total = new BarTotalDataLabels
                        {
                            Style = new BarDataLabelsStyle
                            {
                                FontWeight = "800",
                                Color = IsDarkMode ? "#FFFFFF" : "#000000"
                            }
                        }
                    }
                }
            },
            Yaxis = new List<YAxis>
            {
                new()
                {
                    Min = 0,
                    ForceNiceScale = true
                }
            },
            Theme = new Theme
            {
                Mode = IsDarkMode ? Mode.Dark : Mode.Light
            }
        };
}
