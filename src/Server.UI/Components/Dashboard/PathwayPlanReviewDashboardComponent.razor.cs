using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class PathwayPlanReviewDashboardComponent
{
    [Parameter]
    public string UserId { get; set; } = null!;
    [Parameter]
    public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [Parameter]
    public bool ShowOverdueOnly { get; set; }

    [Parameter]
    public EventCallback<bool> ShowOverdueOnlyChanged { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private GetPathwayPlans.Query Query { get; set; } = null!;

    protected override IQuery<Result<GetPathwayPlans.PathwayPlanDto>> CreateQuery()
     => new GetPathwayPlans.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId
     };

    private ApexChartOptions<GetPathwayPlans.LocationDetail> Options => new()
    {
        Chart = new Chart
        {
            Stacked = true,
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
                    Csv = new ExportCSV { Filename = "PathwayPlanReview-Chart" },
                    Png = new ExportPng { Filename = "PathwayPlanReview-Chart" },
                    Svg = new ExportSvg { Filename = "PathwayPlanReview-Chart" }
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
                Horizontal=false,
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
        Yaxis = new List<YAxis>
        {
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        },
        Responsive =
        [
            new()
            {
                Breakpoint = 768,
                Options = new ApexChartOptions<GetPathwayPlans.LocationDetail>
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
        Colors = new List<string> { "#5cb85c", "#d9534f" }
    };

    private IEnumerable<GetPathwayPlans.PathwayPlanReviewTabularData> FilteredTabularData =>
        ShowOverdueOnly && Data is not null 
            ? Data.TabularData.Where(x => x.IsOverdue) 
            : Data?.TabularData ?? Enumerable.Empty<GetPathwayPlans.PathwayPlanReviewTabularData>();

    private GetPathwayPlans.LocationDetail[] FilteredChartData =>
        ShowOverdueOnly && Data is not null
            ? Data.ChartData.Select(x => new GetPathwayPlans.LocationDetail(
                x.LocationName, 
                x.LocationType, 
                x.OverdueCount, 
                x.OverdueCount))
              .Where(x => x.TotalCount > 0)
              .ToArray()
            : Data?.ChartData ?? Array.Empty<GetPathwayPlans.LocationDetail>();

    private int CustodyCount =>
        ShowOverdueOnly && Data is not null
            ? Data.ChartData.Where(d => d.LocationType?.IsCustody == true).Sum(d => d.OverdueCount)
            : Data?.Custody ?? 0;

    private int CommunityCount =>
        ShowOverdueOnly && Data is not null
            ? Data.ChartData.Where(d => d.LocationType?.IsCommunity == true).Sum(d => d.OverdueCount)
            : Data?.Community ?? 0;

    private async Task OnShowOverdueOnlyChanged(bool value)
    {
        ShowOverdueOnly = value;

        if (ShowOverdueOnlyChanged.HasDelegate)
        {
            await ShowOverdueOnlyChanged.InvokeAsync(value);
        }
    }

    private async Task ShowReviewNotes(string notes)
    {
        var parameters = new DialogParameters
        {
            { nameof(ReviewNotesDialog.Notes), notes }
        };
        
        var options = new DialogOptions 
        { 
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
        
        await DialogService.ShowAsync<ReviewNotesDialog>("Review Notes", parameters, options);
    }
}
