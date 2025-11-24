using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class CasesPerLocationDashboardComponent
{

    [Parameter]
    public string UserId { get; set; } = null!;
    [Parameter]
    public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    
    protected override IRequest<Result<GetCasesPerLocation.CasesPerLocationDto>> CreateQuery()
     => new GetCasesPerLocation.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId
     };

    private ApexChartOptions<GetCasesPerLocation.LocationDetail> Options => new()
    {
        Chart = new ApexCharts.Chart
        {
            Stacked = true,
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
                            Color = IsDarkMode ? "#FFFFFF" :  "#000000", 
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
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        }        
    };

    private List<GetCasesPerLocation.LocationDetail> ChartData()
    {
        if (Data is null)
        {
            throw new InvalidOperationException("Call to ChartData() when no data set");
        }

        var locations = Data.Records.Select(x => x.LocationName).Distinct().ToList();
        var statuses = Data.Records.Select(x => x.Status).Distinct().ToList();

        return locations.SelectMany(location =>
            statuses.Select(status => new GetCasesPerLocation.LocationDetail(location,
                            Data.Records
                            .Where(x => x.LocationName == location)
                            .Select(d => d.LocationType)
                            .FirstOrDefault()!,
                        status, 
                        Data.Records
                            .Where(x => x.LocationName == location && x.Status == status)
                            .Sum(x => x.Count))))
            .ToList();
          
    }
}