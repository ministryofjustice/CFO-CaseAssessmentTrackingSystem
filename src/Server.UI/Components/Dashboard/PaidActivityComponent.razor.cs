using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using ApexCharts;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class PaidActivityComponent
{
    private bool _downloading;

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

    private List<ActivityType> _desiredOrder =
    [
        ActivityType.SupportWork,
        ActivityType.HumanCitizenship,
        ActivityType.CommunityAndSocial,
        ActivityType.InterventionsAndServicesWraparoundSupport,
        ActivityType.EducationAndTraining,
        ActivityType.Employment
    ];
    private ApexChartOptions<LocationActivityCount> _chartOptions = new();

    protected override IQuery<Result<GetPaidActivities.PaidActivitiesDto>> CreateQuery()
     => new GetPaidActivities.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId,
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
        _chartOptions = new ApexChartOptions<LocationActivityCount>
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
                    Horizontal = false,
                    DataLabels = new PlotOptionsBarDataLabels
                    {
                        Total = new BarTotalDataLabels
                        {
                            Enabled = true,
                            Style = new BarDataLabelsStyle
                            {
                                FontWeight = "800",
                                Color = IsDarkMode ? "#FFFFFF" : "#000000",
                            }
                        }
                    },
                }
            },
            DataLabels = new DataLabels
            {
                Enabled = false
            },
            Xaxis = new XAxis
            {
                Title = new AxisTitle { Text = "Payable" }
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
        if (Data?.Details == null){
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

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportActivitiesDashboard.Command()
            {
                Request = new ExportActivitiesDashboard.ActivitiesDashboardExportRequest
                {
                    StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
                    EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set"),
                    TenantId = TenantId,
                    UserId = UserId
                }
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add("An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}
