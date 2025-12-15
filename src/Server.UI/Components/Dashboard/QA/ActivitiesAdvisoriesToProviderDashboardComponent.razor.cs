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
        PlotOptions = new ApexCharts.PlotOptions
        {
            Bar = new ApexCharts.PlotOptionsBar
            {
                Horizontal = false
            }
        },
        Chart = new ApexCharts.Chart
        {
            Stacked = false
        },
        Theme = new ApexCharts.Theme
        {
            Mode = IsDarkMode ? ApexCharts.Mode.Dark : ApexCharts.Mode.Light
        }
    };

}
