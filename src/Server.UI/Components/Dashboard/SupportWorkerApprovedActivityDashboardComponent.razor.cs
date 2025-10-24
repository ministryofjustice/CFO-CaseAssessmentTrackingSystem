using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class SupportWorkerApprovedActivityDashboardComponent
{
    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }

    [EditorRequired, Parameter]
    public string UserId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private ApexChartOptions<LocationActivityCount> chartOptions = new();

    protected override IRequest<Result<GetApprovedActivitiesPerSupportWorker.ApprovedActivitiesPerSupportWorkerDto>> CreateQuery()
     => new GetApprovedActivitiesPerSupportWorker.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Data is null)
        {
            return;
        }

        // Configure chart options
        chartOptions = new ApexChartOptions<LocationActivityCount>
        {
            Chart = new Chart
            {
                Type = ApexCharts.ChartType.Bar,
                Stacked = true,
                StackType = StackType.Normal,
                Toolbar = new Toolbar { Show = true }
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
                Enabled = false
            },
            Xaxis = new XAxis
            {
                Title = new AxisTitle { Text = "Approved" }
            },
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Min = 0,
                    ForceNiceScale = true
                }
            },
            Legend = new Legend
            {
                Position = LegendPosition.Bottom,
                HorizontalAlign = ApexCharts.Align.Center
            },
            Theme = new Theme
            {
                Mode = IsDarkMode ? Mode.Dark : Mode.Light
            },
            Colors = ActivityType.List.Select(at => at.Colour).ToList()
        };
    }

    private List<LocationActivityCount> GetSeriesDataForActivityType(ActivityType activityType)
    {
        if (Data?.Details == null)
        {
            return new List<LocationActivityCount>();
        }

        // Get all unique locations
        var allLocations = Data.Details
            .Select(d => d.LocationName)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        // For each location, get the count for this activity type
        var result = new List<LocationActivityCount>();
        foreach (var location in allLocations)
        {
            var count = Data.Details
                .Where(d => d.LocationName == location && d.ActivityType == activityType)
                .Sum(d => d.Count);

            result.Add(new LocationActivityCount
            {
                Location = location,
                Count = count
            });
        }

        return result;
    }

    public class LocationActivityCount
    {
        public string Location { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}