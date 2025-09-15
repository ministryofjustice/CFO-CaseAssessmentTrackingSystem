using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Domain.Common.Enums;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class CasesPerLocationPerSupportWorkerCardComponent
{

    [EditorRequired, Parameter]
    public string UserId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    
    protected override IRequest<Result<CasesPerLocationSupportWorkerDto>> CreateQuery()
     => new GetCasesPerLocationBySupportWorker.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId
     };

    private ApexChartOptions<Details> Options => new()
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
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        },
    };

    private List<Details> ChartData()
    {
        if (Data is null)
        {
            throw new InvalidOperationException("Call to ChartData() when no data set");
        }

        var locations = Data.Records.Select(x => x.Location).Distinct().ToList();
        var statuses = Data.Records.Select(x => x.Status).Distinct().ToList();

        return locations.SelectMany(location =>
            statuses.Select(status => new Details(location, status, Data.Records
                    .Where(x => x.Location == location && x.Status == status)
                    .Sum(x => x.Count))))
            .ToList();
          
    }
}