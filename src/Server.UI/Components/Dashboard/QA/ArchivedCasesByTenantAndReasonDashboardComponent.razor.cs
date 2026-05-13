using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

public partial class ArchivedCasesByTenantAndReasonDashboardComponent
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

    protected override IRequest<Result<GetArchivedCasesByTenantAndReason.ArchivedCasesByTenantAndReasonDto>>
        CreateQuery()
        => new GetArchivedCasesByTenantAndReason.Query
        {
            CurrentUser = CurrentUser,
            UserId = UserId,
            TenantId = TenantId,
            StartDate = DateRange?.Start
                ?? throw new InvalidOperationException("DateRange not set"),
            EndDate = DateRange?.End
                ?? throw new InvalidOperationException("DateRange not set")
        };
    
    // All known reasons in a fixed order so ApexCharts assigns colours consistently,
    // regardless of which reasons have data in the current date range.
    private static readonly IReadOnlyList<string> _fixedReasonOrder =
        ArchiveReason.List
            .Where(r => r != ArchiveReason.None)
            .OrderBy(r => r.Value)
            .Select(r => r.Name)
            .ToList();

    // One distinct colour per reason (positionally matched to _fixedReasonOrder).
    // "Unknown" is always appended last so #888888 covers it.
    private static readonly string[] _reasonColours =
    [
        "#4E79A7", // CaseloadTooHigh
        "#F28E2B", // Deceased
        "#E15759", // LicenceEnd
        "#76B7B2", // MovedOutsideProviderArea
        "#59A14F", // MovedToNonCFO
        "#EDC948", // NoFurtherSupport
        "#B07AA1", // NoRightToLiveWork
        "#FF9DA7", // NoWishToParticipate
        "#9C755F", // NoLongerEngaging
        "#BAB0AC", // PersonalCircumstances
        "#D4A6C8", // Other
        "#888888", // Unknown (fallback)
    ];

    private IEnumerable<(string Reason, IEnumerable<GetArchivedCasesByTenantAndReason.ArchivedCasesChartData> Items)>
        SeriesByReason
    {
        get
        {
            var dataByReason = Data!.ChartData
                .GroupBy(x => x.Reason)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            var reasons = _fixedReasonOrder.AsEnumerable();

            if (dataByReason.ContainsKey("Unknown"))
            {
                reasons = reasons.Append("Unknown");
            }

            return reasons.Select(reason =>
                (reason, dataByReason.TryGetValue(reason, out var items)
                    ? items
                    : Enumerable.Empty<GetArchivedCasesByTenantAndReason.ArchivedCasesChartData>()));
        }
    }
    
    private ApexChartOptions<GetArchivedCasesByTenantAndReason.ArchivedCasesChartData> Options
        => new()
        {
            Colors = _reasonColours.ToList(),
            Chart = new Chart
            {
                Stacked = true
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
                                Color = IsDarkMode ? "#FFFFFF" : "#000000"
                            }
                        }
                    }
                }
            },
            Yaxis = new List<YAxis>
            {
                new()
                {
                    Min = 0,
                    ForceNiceScale = true
                }
            },
            Theme = new Theme
            {
                Mode = IsDarkMode ? Mode.Dark : Mode.Light
            }
        };
}