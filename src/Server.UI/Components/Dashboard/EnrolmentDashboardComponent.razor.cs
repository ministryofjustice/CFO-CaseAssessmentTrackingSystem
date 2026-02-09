using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class EnrolmentDashboardComponent
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

    protected override IRequest<Result<GetEnrolments.EnrolmentDto>> CreateQuery()
     => new GetEnrolments.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    private ApexCharts.ApexChartOptions<GetEnrolments.LocationDetail> Options => new()
    {
        Chart = new ApexCharts.Chart
        {
            Stacked = true,
        },
        PlotOptions = new ApexCharts.PlotOptions
        {
            Bar = new ApexCharts.PlotOptionsBar
            {
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
        }
    };

}