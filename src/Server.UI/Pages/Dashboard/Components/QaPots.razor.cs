using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Microsoft.EntityFrameworkCore;
using Align = ApexCharts.Align;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class QaPots
{
    private bool _loading;
    private int _enrolmentPqa;
    private int _enrolmentQa1;
    private int _enrolmentQa2;
    private int _enrolmentEscalation;
    private int _activityPqa;
    private int _activityQa1;
    private int _activityQa2;
    private int _activityEscalation;
    private List<QueueData> _enrolmentData = new();
    private List<QueueData> _activityData = new();

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private ApexChartOptions<QueueData> Options => new()
    {
        Chart = new Chart
        {
            Height = "100%",
            Stacked = false,
            Toolbar = new ApexCharts.Toolbar
            {
                Show = true,
                Export = new ExportOptions
                {
                    Csv = new ApexCharts.ExportCSV()
                    {
                        Filename = "Participants",
                    },
                    Png = new ExportPng()
                    {
                        Filename = "Participants-Chart"
                    },
                    Svg = new ExportSvg()
                    {
                        Filename = "Participants-Pie-Chart"
                    }
                }
            },
        },
        Xaxis = new XAxis
        {
            Title = new AxisTitle
            {
                Text = "Queue Stage"
            },
            Categories = new List<string> { "PQA", "QA1", "QA2", "Escalation" }
        },
        Yaxis =
        [
            new YAxis
            {
                Title = new AxisTitle
                {
                    Text = "Number of Items"
                },
                Min = 0
            }
        ],
        Legend = new Legend
        {
            Position = LegendPosition.Top,
            HorizontalAlign = Align.Center
        },
        DataLabels = new DataLabels
        {
            Enabled = true
        },
        Responsive =
        [
            new()
            {
                Breakpoint = 768,
                Options = new ApexChartOptions<QueueData>
                {
                    Chart = new Chart
                    {
                        Height = 300
                    },
                    Legend = new Legend
                    {
                        Position = LegendPosition.Bottom
                    }
                }
            }
        ],
        Theme = new Theme()
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        }
    };

    [CascadingParameter] public UserProfile UserProfile { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;

        var unitOfWork = GetNewUnitOfWork();

        var context = unitOfWork.DbContext;
        // pqa, first pass, second pass, escalation
        _enrolmentPqa = await context.EnrolmentPqaQueue.CountAsync(e => e.IsCompleted == false);
        _enrolmentQa1 = await context.EnrolmentQa1Queue.CountAsync(e => e.IsCompleted == false);
        _enrolmentQa2 = await context.EnrolmentQa2Queue.CountAsync(e => e.IsCompleted == false);
        _enrolmentEscalation = await context.EnrolmentEscalationQueue.CountAsync(e => e.IsCompleted == false);

        // pqa, first pass, second pass, escalation
        _activityPqa = await context.ActivityPqaQueue.CountAsync(e => e.IsCompleted == false);
        _activityQa1 = await context.ActivityQa1Queue.CountAsync(e => e.IsCompleted == false);
        _activityQa2 = await context.ActivityQa2Queue.CountAsync(e => e.IsCompleted == false);
        _activityEscalation = await context.ActivityEscalationQueue.CountAsync(e => e.IsCompleted == false);

        PrepareChartData();

        _loading = false;
    }

    private void PrepareChartData()
    {
        _enrolmentData =
        [
            new QueueData
            {
                Stage = "PQA",
                Count = _enrolmentPqa
            },
            new QueueData
            {
                Stage = "QA1",
                Count = _enrolmentQa1
            },
            new QueueData
            {
                Stage = "QA2",
                Count = _enrolmentQa2
            },
            new QueueData
            {
                Stage = "Escalation",
                Count = _enrolmentEscalation
            }
        ];

        _activityData =
        [
            new QueueData
            {
                Stage = "PQA",
                Count = _activityPqa
            },
            new QueueData
            {
                Stage = "QA1",
                Count = _activityQa1
            },
            new QueueData
            {
                Stage = "QA2",
                Count = _activityQa2
            },
            new QueueData
            {
                Stage = "Escalation",
                Count = _activityEscalation
            }
        ];
    }

    public class QueueData
    {
        public string Stage { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}