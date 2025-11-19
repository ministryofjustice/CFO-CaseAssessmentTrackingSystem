using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard;
public partial class SupportWorkerEmploymentDashboardComponent
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

    protected override IRequest<Result<GetEmploymentsPerSupportWorker.EmploymentsPerSupportWorkerDto>> CreateQuery()
     => new GetEmploymentsPerSupportWorker.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    private ApexCharts.ApexChartOptions<GetEmploymentsPerSupportWorker.LocationDetail> Options => new()
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
        Colors = new List<string> { "#5cb85c", "#d9534f" }
    };

}