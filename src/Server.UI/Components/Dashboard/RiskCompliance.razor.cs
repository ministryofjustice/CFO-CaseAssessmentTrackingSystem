using Cfo.Cats.Application.Features.Compliance.DTOs;
using Cfo.Cats.Application.Features.Compliance.Queries;
using ApexCharts;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class RiskCompliance
{
    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private ApexCharts.ApexChartOptions<RiskComplianceSummaryDto> Options => new()
    {
        Chart = new ApexCharts.Chart
        {
            Stacked = false
        },
        PlotOptions = new ApexCharts.PlotOptions
        {
            Bar = new ApexCharts.PlotOptionsBar
            {
                Horizontal = false
            },
        },
        DataLabels = new ApexCharts.DataLabels
        {
            Enabled = true
        },
        Yaxis = new List<YAxis>
        {
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        },
        Theme = new ApexCharts.Theme
        {
            Mode = IsDarkMode ? ApexCharts.Mode.Dark : ApexCharts.Mode.Light
        },
        Colors = new List<string> { "#FFA500", "#FF6B6B", "#FF4444", "#5cb85c" }
    };

    protected override IRequest<Result<RiskComplianceSummaryDto[]>> CreateQuery()
        => new GetRiskComplianceQuery()
        {
            Date = DateTime.Now.Date
        };
}