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
                    Location = d.Name,
                    Count = d.Payable
                })
                .ToList() ?? new List<ChartDataPoint>();
        }
        else
        {
            return Data?.Details
                .Where(d => d.SupportType == supportType && (d.Count - d.Payable) > 0)
                .Select(d => new ChartDataPoint
                {
                    Location = d.Name,
                    Count = (d.Count - d.Payable)
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
                Toolbar = new Toolbar { Show = false },
                Animations = new Animations { Enabled = true }
            },
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = false
                }
            },
            DataLabels = new DataLabels
            {
                Enabled = true,
                Style = new DataLabelsStyle
                {
                    FontSize = "12px",
                    Colors = new List<string> { "#fff" }
                }
            },
            Xaxis = new XAxis
            {
                Title = new AxisTitle { Text = xAxisTitle }
            },
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Min = 0,
                    ForceNiceScale = true
                }
            },
            Colors = new List<string> { color }
        };
    }

    private record ChartDataPoint
    {
        public string Location { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}