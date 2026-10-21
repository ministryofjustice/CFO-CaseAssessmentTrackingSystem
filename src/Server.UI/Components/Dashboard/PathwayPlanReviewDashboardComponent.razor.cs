using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class PathwayPlanReviewDashboardComponent
{
    [Parameter]
    public string? UserId { get; set; }
    [Parameter]
    public string? TenantId { get; set; }

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [Parameter]
    public bool ShowOverdueOnly { get; set; }

    /// <summary>Optional filter on the participant's current location (null = all).</summary>
    [Parameter]
    public int? ParticipantLocationId { get; set; }

    /// <summary>Optional filter on the location of the latest review (null = all).</summary>
    [Parameter]
    public int? ReviewLocationId { get; set; }

    /// <summary>
    /// Optional filter on the latest review reason value (null = all).
    /// <see cref="PathwayPlanReviewReason.Default"/> matches plans that have not been reviewed yet.
    /// </summary>
    [Parameter]
    public int? ReviewReason { get; set; }

    [Parameter]
    public string? Keyword { get; set; }

    [Parameter]
    public RenderFragment? ToolbarContent { get; set; }

    private const int PageSize = 15;
    private int _currentTablePage;
    private string? _pagingFilterKey;

    private int TotalItems => FilteredTabularData.Count();
    private int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_pagingFilterKey != ChartKey)
        {
            _currentTablePage = 0;
            _pagingFilterKey = ChartKey;
        }
    }

    private void OnPageChanged(int page) => _currentTablePage = page - 1;

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    //private GetPathwayPlans.Query Query { get; set; } = null!;

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
        Colors = ["#5cb85c", "#d9534f"]
    };

    private IEnumerable<GetPathwayPlans.PathwayPlanReviewTabularData> AllRows =>
        Data?.TabularData ?? Enumerable.Empty<GetPathwayPlans.PathwayPlanReviewTabularData>();

    /// <summary>
    /// Rows matching the keyword and location/reason filters (ignores the overdue toggle).
    /// </summary>
    private GetPathwayPlans.PathwayPlanReviewTabularData[] FilteredRows
    {
        get
        {
            var rows = AllRows;

            if (string.IsNullOrWhiteSpace(Keyword) is false)
            {
                var keyword = Keyword.Trim();
                rows = rows.Where(row => MatchesKeyword(row, keyword));
            }

            if (ParticipantLocationId is { } participantLocationId)
            {
                rows = rows.Where(x => x.ParticipantCurrentLocationId == participantLocationId);
            }

            if (ReviewLocationId is { } reviewLocationId)
            {
                rows = rows.Where(x => x.ReviewLocationId == reviewLocationId);
            }

            if (ReviewReason is { } reviewReason)
            {
                rows = rows.Where(x => (x.ReviewReason ?? PathwayPlanReviewReason.Default).Value == reviewReason);
            }

            return rows.ToArray();
        }
    }

    private static bool MatchesKeyword(GetPathwayPlans.PathwayPlanReviewTabularData row, string keyword)
    {
        var reason = row.ReviewReason is null || row.ReviewReason == PathwayPlanReviewReason.Default
            ? "Not reviewed yet"
            : row.ReviewReason.Name;

        return ContainsKeyword(row.ParticipantName, keyword)
            || ContainsKeyword(row.ParticipantId, keyword)
            || ContainsKeyword(row.ParticipantCurrentLocation, keyword)
            || ContainsKeyword(row.ReviewLocation, keyword)
            || ContainsKeyword(reason, keyword)
            || ContainsKeyword(row.ReviewedBy, keyword);
    }

    private static bool ContainsKeyword(string? value, string keyword) =>
        value?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true;

    private IEnumerable<GetPathwayPlans.PathwayPlanReviewTabularData> FilteredTabularData =>
        ShowOverdueOnly
            ? FilteredRows.Where(x => x.IsOverdue)
            : FilteredRows;

    private GetPathwayPlans.LocationDetail[] ChartData => FilteredRows
        .GroupBy(td => new { td.ParticipantCurrentLocation, td.ParticipantCurrentLocationType })
        .OrderBy(g => g.Key.ParticipantCurrentLocation)
        .ThenBy(g => g.Key.ParticipantCurrentLocationType)
        .Select(g => new GetPathwayPlans.LocationDetail(
            g.Key.ParticipantCurrentLocation!,
            g.Key.ParticipantCurrentLocationType,
            g.Count(),
            g.Count(d => d.IsOverdue)))
        .ToArray();

    private GetPathwayPlans.LocationDetail[] FilteredChartData =>
        ShowOverdueOnly
            ? ChartData.Select(x => new GetPathwayPlans.LocationDetail(
                x.LocationName,
                x.LocationType,
                x.OverdueCount,
                x.OverdueCount))
              .Where(x => x.TotalCount > 0)
              .ToArray()
            : ChartData;

    private int TotalReviewed => FilteredRows.Count(d => d.IsOverdue == false);

    private int TotalOverdueCount => FilteredRows.Count(d => d.IsOverdue);

    private int CustodyCount =>
        FilteredTabularData.Count(d => d.ParticipantCurrentLocationType?.IsCustody == true);

    private int CommunityCount =>
        FilteredTabularData.Count(d => d.ParticipantCurrentLocationType?.IsCommunity == true);

    private string ChartKey =>
        $"{ShowOverdueOnly}|{ParticipantLocationId}|{ReviewLocationId}|{ReviewReason}|{Keyword}";

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
