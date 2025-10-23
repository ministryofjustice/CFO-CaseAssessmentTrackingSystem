using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using System.Linq;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class CasesPerLocationPerSupportWorkerCardComponent
{

    [EditorRequired, Parameter]
    public string UserId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    
    protected override IRequest<Result<GetCasesPerLocationBySupportWorker.CasesPerLocationSupportWorkerDto>> CreateQuery()
     => new GetCasesPerLocationBySupportWorker.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId
     };

    private ApexChartOptions<GetCasesPerLocationBySupportWorker.LocationDetail> Options => new()
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

    private List<GetCasesPerLocationBySupportWorker.LocationDetail> ChartData()
    {
        if (Data is null)
        {
            throw new InvalidOperationException("Call to ChartData() when no data set");
        }

        var locations = Data.Records.Select(x => x.LocationName).Distinct().ToList();
        var statuses = Data.Records.Select(x => x.Status).Distinct().ToList();

        return locations.SelectMany(location =>
            statuses.Select(status => new GetCasesPerLocationBySupportWorker.LocationDetail(location,
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