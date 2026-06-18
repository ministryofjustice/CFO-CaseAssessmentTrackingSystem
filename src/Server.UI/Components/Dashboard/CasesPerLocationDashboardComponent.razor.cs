using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class CasesPerLocationDashboardComponent
{
    [Parameter]
    public string UserId { get; set; } = null!;
    [Parameter]
    public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }
    
    private bool _downloading;
    
    protected override IRequest<Result<GetCasesPerLocation.CasesPerLocationDto>> CreateQuery()
     => new GetCasesPerLocation.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId
     };

    private ApexChartOptions<GetCasesPerLocation.LocationDetail> Options => new()
    {
        Chart = new Chart
        {
            Stacked = true,
        },
        
        Legend = new Legend
        {
            Show = true,
            ShowForSingleSeries = true
        },
        
        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar
            {
                DataLabels = new PlotOptionsBarDataLabels
                {
                    Total = new BarTotalDataLabels
                    {
                        Style = new BarDataLabelsStyle
                        {
                            FontWeight = "800",
                            Color = IsDarkMode ? "#FFFFFF" :  "#000000", 
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

    private List<GetCasesPerLocation.LocationDetail> ChartData()
    {
        if (Data is null)
        {
            throw new InvalidOperationException("Call to ChartData() when no data set");
        }

        var locations = Data.Records.Select(x => x.LocationName).Distinct().ToList();
        var statuses = Data.Records.Select(x => x.Status).Distinct().ToList();

        return locations.SelectMany(location =>
            statuses.Select(status => new GetCasesPerLocation.LocationDetail(location,
                            Data.Records
                            .Where(x => x.LocationName == location)
                            .Select(d => d.LocationType)
                            .FirstOrDefault()!,
                        status, 
                        Data.Records
                            .Where(x => x.LocationName == location && x.Status == status)
                            .Sum(x => x.Count))))
            .ToList();
    }
    
    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await Service.Send(new ExportCasesPerLocationDashboard.Command
            {
                Request = new ExportCasesPerLocationDashboard.CasesPerLocationDashboardExportRequest
                {
                    UserId = UserId,
                    TenantId = TenantId
                }
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error has occurred while generating the cases per location dashboard export");
            Snackbar.Add("An error has occurred while generating the cases per location dashboard export.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}
