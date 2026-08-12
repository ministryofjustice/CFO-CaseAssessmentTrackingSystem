using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Workspaces.Performance.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class RecentlyApprovedActivitiesComponent : CatsComponent<GetRecentlyApprovedActivities.ApprovedActivitiesDto>
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

    private bool _downloading;

    protected override IQuery<Result<GetRecentlyApprovedActivities.ApprovedActivitiesDto>> CreateQuery()
        => new GetRecentlyApprovedActivities.Query()
        {
            CurrentUser = CurrentUser,
            UserId = UserId,
            TenantId = TenantId,
            StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
            EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
        };

    private ApexChartOptions<GetRecentlyApprovedActivities.ActivityDetail> Options => new()
    {
        Chart = new Chart
        {
            Stacked = false,
            Toolbar = new Toolbar
            {
                Show = true,
                Tools = new Tools
                {
                    Download = true,
                    Selection = false,
                    Zoom = false,
                    Zoomin = false,
                    Zoomout = false,
                    Pan = false,
                    Reset = false
                },
                Export = new ExportOptions
                {
                    Csv = new ExportCSV { Filename = "RecentlyApprovedActivities-Chart" },
                    Png = new ExportPng { Filename = "RecentlyApprovedActivities-Chart" },
                    Svg = new ExportSvg { Filename = "RecentlyApprovedActivities-Chart" }
                }
            }
        },
        Legend = new Legend
        {
            Show = true,
            ShowForSingleSeries = true,
            Position = LegendPosition.Top,
            HorizontalAlign = ApexCharts.Align.Center
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
            },
        },
        Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Rotate = -45,
                RotateAlways = true,
                OffsetY = 5
            }
        },
        Yaxis =
        [
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        ],
        Responsive =
        [
            new()
            {
                Breakpoint = 768,
                Options = new ApexChartOptions<GetRecentlyApprovedActivities.ActivityDetail>
                {
                    Legend = new Legend
                    {
                        Position = LegendPosition.Bottom
                    }
                }
            }
        ],
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        },
        Colors = ["#5cb85c", "#d9534f"]
    };

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportRecentApprovedActivities.Command()
            {
                Request = new ExportRecentApprovedActivities.RecentApprovedActivitiesExportRequest
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
