using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class InitiativeObjectivesDashboardComponent
{
    [Parameter]
    public string? UserId { get; set; }
    [Parameter]
    public string? TenantId { get; set; }

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private InitiativeDto? _initiativeFilter;
    private bool _downloading;
    private bool ShowActiveOnly { get; set; } = false;

    protected override IRequest<Result<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto[]>> CreateQuery()
     => new GetInitiativeObjectivesDashboard.Query
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId
     };

    private ApexChartOptions<InitiativeChartPoint> Options => new()
    {
        Chart = new Chart
        {
            Stacked = true
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
        Yaxis = new List<YAxis>
        {
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        },
        Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Rotate = -45,
                RotateAlways = true
            }
        },
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        },
        Colors = new List<string> { "#1976d2", "#5cb85c" }
    };

    private IEnumerable<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto> FilteredRows =>
        Data is null
            ? Array.Empty<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto>()
            : Data
                .Where(r => _initiativeFilter is null || r.InitiativeCode == _initiativeFilter.Code)
                .Where(r => !ShowActiveOnly || !r.IsObjectiveCompleted);

    public record InitiativeChartPoint(string InitiativeCode, int ActiveCount, int CompletedCount);

    private int TotalActive => Data?.Count(r => !r.IsObjectiveCompleted) ?? 0;

    private int TotalCompleted => Data?.Count(r => r.IsObjectiveCompleted) ?? 0;

    private InitiativeChartPoint[] ChartData =>
        Data?
            .GroupBy(r => r.InitiativeCode)
            .Select(g => new InitiativeChartPoint(
                g.Key,
                g.Count(r => !r.IsObjectiveCompleted),
                g.Count(r => r.IsObjectiveCompleted)))
            .OrderBy(p => p.InitiativeCode)
            .ToArray()
        ?? Array.Empty<InitiativeChartPoint>();

    private InitiativeChartPoint[] FilteredChartData =>
        ShowActiveOnly && Data is not null
            ? ChartData.Where(x => x.ActiveCount > 0).ToArray()
            : ChartData ?? Array.Empty<InitiativeChartPoint>();

    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await Service.Send(new ExportInitiativeObjectivesDashboard.Command
            {
                Request = new ExportInitiativeObjectivesDashboard.InitiativeObjectivesDashboardExportRequest
                {
                    UserId = UserId,
                    TenantId = TenantId,
                    InitiativeCode = _initiativeFilter?.Code,
                    ShowActiveOnly = ShowActiveOnly
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
            Logger.LogError(ex, "An error has occurred while generating the initiative objectives dashboard export");
            Snackbar.Add("An error has occurred while generating the initiative objectives dashboard export.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}
