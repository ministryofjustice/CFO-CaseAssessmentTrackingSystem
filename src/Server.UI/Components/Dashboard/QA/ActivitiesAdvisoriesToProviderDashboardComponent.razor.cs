using ActualLab.Fusion.Authentication;
using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

public partial class ActivitiesAdvisoriesToProviderDashboardComponent
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

    protected override IRequest<Result<GetActivitiesAdvisoriesToProvider.ActivitiesAdvisoriesToProviderDto>> CreateQuery()
     => new GetActivitiesAdvisoriesToProvider.Query()
     {
         CurrentUser = CurrentUser,
         UserId = CurrentUser.UserId,
         TenantId = TenantId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    private ApexCharts.ApexChartOptions<GetActivitiesAdvisoriesToProvider.ActivitiesAdvisoriesChartData> Options => new()
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
        DataLabels = new ApexCharts.DataLabels
        {
            Enabled = false
        },
        Xaxis = new ApexCharts.XAxis
        {
            Labels = new ApexCharts.XAxisLabels
            {
                Rotate = -45,
                RotateAlways = true,
                Trim = false
            }
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
