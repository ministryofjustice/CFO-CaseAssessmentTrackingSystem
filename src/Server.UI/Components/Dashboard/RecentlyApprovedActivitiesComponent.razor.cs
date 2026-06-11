using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;

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

    protected override IRequest<Result<GetRecentlyApprovedActivities.ApprovedActivitiesDto>> CreateQuery()
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
            Stacked = false
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
        Yaxis =
        [
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        ],
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        },
        Colors = ["#5cb85c", "#d9534f"]
    };

    private void EditParticipant(string participantId) => Navigation.NavigateTo($"/pages/participants/{participantId}");
}
