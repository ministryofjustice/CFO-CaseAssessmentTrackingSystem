using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using FluentValidation;

namespace Cfo.Cats.Server.UI.Components.Dashboard;
public partial class SupportWorkerPRIDashboardComponent
{
    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }

    [EditorRequired, Parameter]
    public string UserId { get; set; } = null!;
    [Parameter]
    public bool VisualMode { get; set; } = true;

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    protected override IRequest<Result<GetPrisPerSupportWorker.PrisPerSupportWorkerDto>> CreateQuery()
     => new GetPrisPerSupportWorker.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    private List<ChartDataPoint> GetChartData(string supportType, bool isPayable)
    {
        if (isPayable) {
            return Data?.Details
                .Where(d => d.SupportType == supportType && d.Payable > 0)
                .Select(d => new ChartDataPoint
                {
                    Location = d.LocationName,
                    Count = d.Payable
                })
                .ToList() ?? new List<ChartDataPoint>();
        }
        else
        {
            return Data?.Details
                .Where(d => d.SupportType == supportType && (d.TotalCount - d.Payable) > 0)
                .Select(d => new ChartDataPoint
                {
                    Location = d.LocationName,
                    Count = (d.TotalCount - d.Payable)
                })
                .ToList() ?? new List<ChartDataPoint>();
        }

    }

    private ApexChartOptions<ChartDataPoint> GetChartOptions(string color, string xAxisTitle)
    {
        return new ApexChartOptions<ChartDataPoint>
        {
            Chart = new Chart
            {
                Stacked = true,
                Toolbar = new Toolbar { Show = false },
                Animations = new Animations { Enabled = true }
            },
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = false,
                    ColumnWidth = "55%",
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
                }
            },
            Xaxis = new XAxis
            {
                Title = new AxisTitle { Text = xAxisTitle },
                Labels = new XAxisLabels { Rotate = -45 }
            },
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Min = 0,
                    ForceNiceScale = true,
                    Labels = new YAxisLabels
                    {
                        Show = true
                    }
                }
            },
            Colors = new List<string> { color },
            Tooltip = new Tooltip
            {
                Enabled = true
            }
        };
    }

    private record ChartDataPoint
    {
        public string Location { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}