using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;
public partial class SupportAndReferralDashboardComponent
{
    private bool _downloading;

    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }

    [Parameter]
    public string UserId { get; set; } = null!;

    [Parameter]
    public string TenantId { get; set; } = null!;
    
    [Parameter]
    public bool VisualMode { get; set; } = true;

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    protected override IQuery<Result<GetSupportReferrals.SupportAndReferralDto>> CreateQuery()
     => new GetSupportReferrals.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    private List<ChartDataPoint> GetChartData(string supportType) => Data?.Details
                .Where(d => d.SupportType == supportType && d.Payable > 0)
                .Select(d => new ChartDataPoint
                {
                    Location = d.LocationName,
                    Count = d.Payable
                })
                .ToList() ?? new List<ChartDataPoint>();

    private ApexChartOptions<ChartDataPoint> GetChartOptions(string color, string xAxisTitle) =>
        new()
        {
            Chart = new Chart
            {
                Stacked = true,
                Toolbar = new Toolbar { Show = false },
                Animations = new Animations { Enabled = true }
            },
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = false,
                    ColumnWidth = "55%",
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
            Xaxis = new XAxis
            {
                Title = new AxisTitle { Text = xAxisTitle },
                Labels = new XAxisLabels { Rotate = -45 }
            },
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Min = 0,
                    ForceNiceScale = true,
                    Labels = new YAxisLabels
                    {
                        Show = true
                    }
                }
            },
            Colors = new List<string> { color },
            Tooltip = new Tooltip
            {
                Enabled = true
            }
        };

    private record ChartDataPoint
    {
        public string Location { get; init; } = string.Empty;
        public int Count { get; init; }
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportSupportAndReferralDashboard.Command()
            {
                Request = new ExportSupportAndReferralDashboard.SupportAndReferralDashboardExportRequest
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
