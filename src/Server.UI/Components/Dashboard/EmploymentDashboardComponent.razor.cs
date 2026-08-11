using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Workspaces.Performance.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class EmploymentDashboardComponent
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
    
    protected override IQuery<Result<GetEmployments.EmploymentsDto>> CreateQuery()
     => new GetEmployments.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    private ApexChartOptions<GetEmployments.LocationDetail> Options => new()
    {
        Chart = new Chart
        {
            Stacked = true,
            Toolbar = new Toolbar { Show = false }
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
                OffsetY = 0
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
                Options = new ApexChartOptions<GetEmployments.LocationDetail>
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
            var result = await Service.Send(new ExportEmployments.Command()
            {
                Request = new ExportEmployments.EmploymentExportRequest
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
