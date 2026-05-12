using Cfo.Cats.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Components.Authorization;
using ApexCharts;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

public partial class EnrolmentsFeedbackSeniorComponent
{
    [EditorRequired, Parameter] public DateRange? DateRange { get; set; }

    [Parameter] public string UserId { get; set; } = null!;

    [Parameter] public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter] public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private bool HasReadItems => Data?.TabularData?.Any(x => x.IsRead) ?? false;
    
    protected override IRequest<Result<GetEnrolmentsFeedback.EnrolmentsFeedbackDto>>
        CreateQuery()
        => new GetEnrolmentsFeedback.Query
        {
            CurrentUser = CurrentUser,
            UserId = UserId,
            TenantId = TenantId,
            StartDate = DateRange?.Start
                        ?? throw new InvalidOperationException("DateRange not set"),
            EndDate = DateRange?.End
                      ?? throw new InvalidOperationException("DateRange not set"),
            ShowRead = ShowRead
        };

    private List<ChartSeries> SeriesByReason
    {
        get
        {
            var data = Data?.ChartData ?? Array.Empty<GetEnrolmentsFeedback.EnrolmentsFeedbackChartData>();

            var recipients = data
                .Select(x => x.Recipient ?? "Unknown")
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var reasons = data
                .Select(x => x.EnrolmentFeedbackReason ?? "Unknown")
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return reasons.Select(reason => new ChartSeries
            {
                Reason = reason,
                Items = recipients.Select(recipient =>
                {
                    var match = data.FirstOrDefault(x =>
                        (x.Recipient ?? "Unknown") == recipient &&
                        (x.EnrolmentFeedbackReason ?? "Unknown") == reason);

                    return new GetEnrolmentsFeedback.EnrolmentsFeedbackChartData
                    {
                        Recipient = recipient,
                        EnrolmentFeedbackReason = reason,
                        Count = match?.Count ?? 0
                    };
                }).ToList()
            }).ToList();
        }
    }

    private class ChartSeries
    {
        public string Reason { get; init; } = null!;
        public List<GetEnrolmentsFeedback.EnrolmentsFeedbackChartData> Items { get; init; } = [];
    }

    private ApexChartOptions<GetEnrolmentsFeedback.EnrolmentsFeedbackChartData> Options
        => new()
        {
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
    
    private bool _showRead;
    
    private bool ShowRead
    {
        get => _showRead;
        set
        {
            if (_showRead == value)
            {
                return;
            }

            _showRead = value;
            StateHasChanged();
        }
    }
    
    private static Color GetUnreadColour(DateTime? created, bool isRead)
    {
        if (isRead)
        {
            return Color.Success;
        }

        var isOlderThanTwoWeeks = created <= DateTime.Now.AddDays(-14);

        if (isOlderThanTwoWeeks)
        {
            return Color.Error;
        }

        return Color.Warning;
    }
    
    private IEnumerable<GetEnrolmentsFeedback.EnrolmentsFeedbackTabularData> FilteredData
    {
        get
        {
            var data = Data?.TabularData ?? Array.Empty<GetEnrolmentsFeedback.EnrolmentsFeedbackTabularData>();

            if (ShowRead)
            {
                return data;
            }

            return data.Where(x => !x.IsRead);
        }
    }
}
