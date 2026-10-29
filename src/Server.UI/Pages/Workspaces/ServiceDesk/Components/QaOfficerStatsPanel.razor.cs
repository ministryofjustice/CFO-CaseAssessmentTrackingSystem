using ApexCharts;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Components;

public partial class QaOfficerStatsPanel
{
    private const int PageSize = 15;
    private const string Qa1 = "QA1";
    private const string Qa2 = "QA2";
    private const string Esc = "Esc";
    private static readonly Dictionary<string, string> QueueColours = new()
    {
        [Qa1] = "#1E88E5", // Blue
        [Qa2] = "#FB8C00", // Orange
        [Esc] = "#E53935"  // Red
    };
    private static readonly string[] QueueNames = [.. QueueColours.Keys];
    private MudDateRangePicker _picker = null!;
    private DateRange _dateRange = new(DateTime.Today, DateTime.Today);
    private QaStatsQueue? _queue;
    private List<QaOfficerDto> _users = [];
    private string _crn = string.Empty;
    private string _qaUserId = string.Empty;
    private string _isAccepted = string.Empty;
    private bool _notesOnly;
    private bool _downloading;
    private int _pageNumber = 1;
    private string _orderBy = nameof(QaOfficerStatDto.LastModified);
    private bool _descending = true;
    private int _totalItems;
    private List<QaOfficerStatDto> _pageRows = [];
    private int _totalPages = 1;
    private int _chartKey;
    private string[] _chartQueues = QueueNames;
    private List<ChartPoint> _dailyPoints = [];
    private List<ChartPoint> _userPoints = [];
    private List<ChartPoint> _pieData = [];
    private List<UserSummary> _summaries = [];
    private bool _hasChartData;

    [Parameter, EditorRequired]
    public QaStatsProcess Process { get; set; }

    [Parameter]
    public DateRange? DateRange { get; set; }

    [Parameter]
    public EventCallback<DateRange> DateRangeChanged { get; set; }

    [Parameter]
    public bool VisualMode { get; set; } = true;

    [Parameter]
    public EventCallback<bool> VisualModeChanged { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _dateRange = DateRange ?? _dateRange;
        await LoadUsersAsync();
        await base.OnInitializedAsync();
    }

    protected override IQuery<Result<QaOfficerStatsDto>> CreateQuery() => new GetQaOfficerStats.Query
    {
        Filter = BuildFilter(),
        VisualMode = VisualMode,
        PageNumber = _pageNumber,
        PageSize = PageSize,
        OrderBy = _orderBy,
        SortDirection = _descending ? "Descending" : "Ascending"
    };

    protected override void OnDataLoaded(QaOfficerStatsDto data)
    {
        base.OnDataLoaded(data);

        if (VisualMode)
        {
            BuildCharts(data.Summary);
        }
        else
        {
            _pageRows = data.Rows?.Items.ToList() ?? [];
            _totalItems = data.Rows?.TotalItems ?? 0;
            _totalPages = Math.Max(1, data.Rows?.TotalPages ?? 1);
        }
    }

    private async Task LoadUsersAsync()
    {
        var start = (_dateRange.Start ?? DateTime.Today).Date;

        var result = await Service.Send(new GetQaOfficerStatsUsers.Query
        {
            Process = Process,
            StartDate = start,
            EndDate = (_dateRange.End ?? start).Date
        });

        _users = result.Succeeded && result.Data is not null ? result.Data : [];

        if (_qaUserId.Length > 0 && _users.All(u => u.UserId != _qaUserId))
        {
            _qaUserId = string.Empty;
        }
    }

    private void BuildCharts(List<QaOfficerStatSummaryDto> summary)
    {
        _chartQueues = _queue is { } queue ? [QueueName(queue)] : QueueNames;

        var start = (_dateRange.Start ?? DateTime.Today).Date;
        var end = (_dateRange.End ?? start).Date;

        var perDay = summary
            .GroupBy(x => (x.Day.Date, x.Queue))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

        _dailyPoints = Enumerable.Range(0, (end - start).Days + 1)
            .Select(offset => start.AddDays(offset))
            .SelectMany(day => _chartQueues.Select(queueName => new ChartPoint(
                day.ToString("dd MMM"),
                queueName,
                perDay.GetValueOrDefault((day, queueName)))))
            .ToList();

        var perUser = summary
            .GroupBy(x => (User: x.QaUser ?? "Unknown", x.Queue))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

        var userNames = summary
            .Select(x => x.QaUser ?? "Unknown")
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        _userPoints = userNames
            .SelectMany(user => _chartQueues.Select(queueName => new ChartPoint(
                user,
                queueName,
                perUser.GetValueOrDefault((user, queueName)))))
            .ToList();

        _summaries = userNames
            .Select(user => new UserSummary(
                user,
                perUser.GetValueOrDefault((user, Qa1)),
                perUser.GetValueOrDefault((user, Qa2)),
                perUser.GetValueOrDefault((user, Esc))))
            .ToList();

        _pieData = _chartQueues
            .Select(queueName => new ChartPoint(queueName, queueName, summary.Where(x => x.Queue == queueName).Sum(x => x.Count)))
            .ToList();

        _hasChartData = summary.Count > 0;
        _chartKey++;
    }

    private QaOfficerStatsFilter BuildFilter()
    {
        var start = (_dateRange.Start ?? DateTime.Today).Date;

        return new QaOfficerStatsFilter
        {
            Process = Process,
            StartDate = start,
            EndDate = (_dateRange.End ?? start).Date,
            Queues = _queue is { } queue ? [queue] : [],
            QaUserId = string.IsNullOrEmpty(_qaUserId) ? null : _qaUserId,
            IsAccepted = VisualMode ? null : _isAccepted switch
            {
                "yes" => true,
                "no" => false,
                _ => null
            },
            NotesOnly = !VisualMode && _notesOnly,
            Crn = VisualMode || string.IsNullOrWhiteSpace(_crn) ? null : _crn.Trim()
        };
    }

    private string ProcessLabel => Process == QaStatsProcess.Enrolments ? "Enrolments" : "Activities";

    private string SelectedQueueLabel => _queue is { } queue ? QueueLabel(queue) : "All Queues";

    private string SelectedUserLabel => _users.FirstOrDefault(u => u.UserId == _qaUserId)?.DisplayName ?? "All QA Users";

    private string SelectedIsAcceptedLabel => _isAccepted switch
    {
        "yes" => "Accepted: Yes",
        "no" => "Accepted: No",
        _ => "Accepted: All"
    };

    private Task ApplyFilters()
    {
        _pageNumber = 1;
        return LoadDataAsync();
    }

    private Task ClearFilters()
    {
        _crn = string.Empty;
        _queue = null;
        _qaUserId = string.Empty;
        _isAccepted = string.Empty;
        _notesOnly = false;
        return ApplyFilters();
    }

    private Task OnCrnChanged(string? crn)
    {
        _crn = crn ?? string.Empty;
        return ApplyFilters();
    }

    private Task OnQueueChanged(QaStatsQueue? queue)
    {
        _queue = queue;
        return ApplyFilters();
    }

    private async Task OnDateRangeChanged(DateRange range)
    {
        _dateRange = range;
        await DateRangeChanged.InvokeAsync(range);
        await LoadUsersAsync();
        _pageNumber = 1;
        await LoadDataAsync();
    }

    private Task OnQaUserChanged(string? userId)
    {
        _qaUserId = userId ?? string.Empty;
        return ApplyFilters();
    }

    private Task OnIsAcceptedChanged(string? value)
    {
        _isAccepted = value ?? string.Empty;
        return ApplyFilters();
    }

    private Task OnNotesOnlyChanged(bool value)
    {
        _notesOnly = value;
        return ApplyFilters();
    }

    private async Task OnVisualModeChanged(bool value)
    {
        VisualMode = value;
        await VisualModeChanged.InvokeAsync(value);
        await ApplyFilters();
    }

    private Task OnPageChanged(int page)
    {
        _pageNumber = page;
        return LoadDataAsync();
    }

    private string SortIcon(string column) => _orderBy != column
        ? string.Empty
        : _descending ? Icons.Material.Filled.ArrowDownward : Icons.Material.Filled.ArrowUpward;

    private Task SortByAsync(string column)
    {
        _descending = _orderBy == column && !_descending;
        _orderBy = column;
        return ApplyFilters();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await Service.Send(new ExportQaOfficerStats.Command
            {
                Filter = BuildFilter(),
                QaUserName = _users.FirstOrDefault(u => u.UserId == _qaUserId)?.DisplayName
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
        catch (Exception)
        {
            Snackbar.Add("An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private ApexChartOptions<ChartPoint> BarOptions(string axisTitle) => new()
    {
        Chart = new Chart { Stacked = true },
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
        Xaxis = new XAxis { Title = new AxisTitle { Text = axisTitle } },
        Yaxis = [new YAxis { Min = 0, ForceNiceScale = true, DecimalsInFloat = 0 }],
        Legend = new Legend { Position = LegendPosition.Top },
        Theme = new Theme { Mode = IsDarkMode ? Mode.Dark : Mode.Light }
    };

    private ApexChartOptions<ChartPoint> PieOptions => new()
    {
        Chart = new Chart { Type = ApexCharts.ChartType.Pie },
        Legend = new Legend { Position = LegendPosition.Bottom },
        Theme = new Theme { Mode = IsDarkMode ? Mode.Dark : Mode.Light }
    };

    private static string QueueColour(string queueName) => QueueColours.GetValueOrDefault(queueName, "#888888");

    private static string QueueName(QaStatsQueue queue) => queue switch
    {
        QaStatsQueue.Qa1 => Qa1,
        QaStatsQueue.Qa2 => Qa2,
        QaStatsQueue.Escalation => Esc,
        _ => throw new ArgumentOutOfRangeException($"Unknown queue {queue}")
    };

    private static string QueueLabel(QaStatsQueue queue) => queue switch
    {
        QaStatsQueue.Qa1 => Qa1,
        QaStatsQueue.Qa2 => Qa2,
        QaStatsQueue.Escalation => "Escalation",
        _ => throw new ArgumentOutOfRangeException($"Unknown queue {queue}")
    };

    private sealed record ChartPoint(string Label, string Queue, int Count);

    private sealed record UserSummary(string User, int Qa1, int Qa2, int Escalation)
    {
        public int Total => Qa1 + Qa2 + Escalation;
    }
}
