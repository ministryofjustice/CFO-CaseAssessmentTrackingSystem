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

    [Parameter]
    public bool ShowActiveOnly { get; set; }

    [Parameter]
    public EventCallback<bool> ShowActiveOnlyChanged { get; set; }

    [Parameter]
    public InitiativeDto? InitiativeFilter { get; set; }

    [Parameter]
    public EventCallback<InitiativeDto?> InitiativeFilterChanged { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private InitiativeDto? _initiativeFilter;
    private bool _downloading;

    protected override void OnParametersSet() => _initiativeFilter = InitiativeFilter;

    protected override IQuery<Result<GetInitiativeObjectivesDashboard.InitiativeObjectiveRowDto[]>> CreateQuery()
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
                RotateAlways = true,
                Trim = true
            },
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

    public record InitiativeChartPoint(string InitiativeCode, string InitiativeDescription, DateTime InitiativeStartDate, int ActiveCount, int CompletedCount);

    private int TotalActive => Data?.Count(r => !r.IsObjectiveCompleted) ?? 0;

    private int TotalCompleted => Data?.Count(r => r.IsObjectiveCompleted) ?? 0;

    private InitiativeChartPoint[] ChartData =>
        Data?
            .GroupBy(r => r.InitiativeCode)
            .Select(g => new InitiativeChartPoint(
                g.Key,
                g.First().InitiativeDescription,
                g.First().InitiativeStartDate,
                g.Count(r => !r.IsObjectiveCompleted),
                g.Count(r => r.IsObjectiveCompleted)))
            .OrderBy(p => p.InitiativeStartDate)
            .ToArray()
        ?? Array.Empty<InitiativeChartPoint>();

    private InitiativeChartPoint[] FilteredChartData =>
        ShowActiveOnly && Data is not null
            ? ChartData.Where(x => x.ActiveCount > 0).ToArray()
            : ChartData ?? Array.Empty<InitiativeChartPoint>();

    private async Task OnShowActiveOnlyChanged(bool value)
    {
        ShowActiveOnly = value;

        if (ShowActiveOnlyChanged.HasDelegate)
        {
            await ShowActiveOnlyChanged.InvokeAsync(value);
        }
    }

    private async Task OnInitiativeFilterChanged()
    {
        if (InitiativeFilterChanged.HasDelegate)
        {
            await InitiativeFilterChanged.InvokeAsync(_initiativeFilter);
        }
    }

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
