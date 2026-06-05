using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using ApexCharts;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class RecentlyApprovedActivitiesComponent : CatsComponent<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>
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
    
    private int _pageNumber = 1;
    private LocationDto? _location;
    private List<ActivityType>? _includeTypes;

    private class ChartDataPoint
    {
        public string Date { get; init; } = string.Empty;
        public string ActivityType { get; init; } = string.Empty;
        public int Count { get; init; }
    }

    private List<ChartDataPoint> ChartData
    {
        get
        {
            if (Data?.Items == null)
            {
                return new List<ChartDataPoint>();
            }

            return Data.Items
                .Where(a => a.ApprovedOn.HasValue)
                .GroupBy(a => new
                {
                    Date = a.ApprovedOn!.Value.Date,
                    Type = a.Definition.Type.Name
                })
                .Select(g => new ChartDataPoint
                {
                    Date = g.Key.Date.ToString("yyyy-MM-dd"),
                    ActivityType = g.Key.Type,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ThenBy(x => x.ActivityType)
                .ToList();
        }
    }

    private ApexChartOptions<ChartDataPoint> Options
        => new()
        {
            Chart = new Chart
            {
                Type = ApexCharts.ChartType.Bar,
                Stacked = true,
                Toolbar = new Toolbar
                {
                    Show = true
                }
            },
            Theme = new Theme
            {
                Mode = IsDarkMode ? Mode.Dark : Mode.Light
            },
            Xaxis = new XAxis
            {
                Title = new AxisTitle
                {
                    Text = "Approval Date"
                }
            },
            Yaxis = new List<YAxis>
            {
                new()
                {
                    Title = new AxisTitle
                    {
                        Text = "Number of Activities"
                    }
                }
            },
            DataLabels = new DataLabels
            {
                Enabled = true
            },
            Legend = new Legend
            {
                Position = LegendPosition.Top
            }
        };

    protected override IRequest<Result<PaginatedData<RecentlyApprovedActivitiesSummaryDto>>> CreateQuery()
        => new GetRecentlyApprovedActivities.Query()
        {
            UserProfile = CurrentUser,
            PageSize = 10,
            OrderBy = "ApprovedOn",
            SortDirection = $"{SortDirection.Descending}",
            PageNumber = _pageNumber,
            Location = _location,
            ApprovedStart = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
            ApprovedEnd = DateRange?.End ?? throw new InvalidOperationException("DateRange not set"),
            IncludeTypes = _includeTypes,
            TenantId = TenantId
        };

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        await base.OnInitializedAsync();
    }

    private Task OnPaginationChanged(int pageNumber)
    {
        _pageNumber = pageNumber;
        return LoadDataAsync();
    }

    private Task OnRefresh()
    {
        _pageNumber = 1; 
        return LoadDataAsync();
    }

    private Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        _includeTypes = types?.ToList();
        return OnRefresh();
    }

    private Task OnLocationChanged() => OnRefresh();

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}
