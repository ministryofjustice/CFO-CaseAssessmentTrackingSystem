using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Components;

public partial class ActivitiesInQaPots
{
    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    protected override IQuery<Result<ActivitiesInQaPotsDto>> CreateQuery()
        => new GetActivitiesInQaPots.Query
        {
            CurrentUser = CurrentUser,
            IncludeTeams = CurrentUser.AssignedRoles is { Length: > 0}
        };

    private IEnumerable<QaPotPoint> ChartPoints => Data is null
        ? []
        : new[]
        {
            new QaPotPoint("Education", Data.EducationPqa, Data.EducationCfo),
            new QaPotPoint("Employment", Data.EmploymentPqa, Data.EmploymentCfo),
            new QaPotPoint("ISWS", Data.IswSupportPqa, Data.IswSupportCfo),
        };

    private ApexChartOptions<QaPotPoint> Options => new()
    {
        Chart = new Chart
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
        }
    };

    private sealed record QaPotPoint(string Category, int ProviderQa, int CfoQa);
}
