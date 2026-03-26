using Cfo.Cats.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Components.Authorization;
using ApexCharts;
using Cfo.Cats.Application.Features.Activities.Commands;

namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

public partial class ActivitiesFeedbackComponent
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
    private bool IsUpdating { get; set; }
 
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    protected override IRequest<Result<GetActivitiesFeedback.ActivitiesFeedbackDto>> CreateQuery()
        => new GetActivitiesFeedback.Query
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
            var data = Data?.ChartData ?? Array.Empty<GetActivitiesFeedback.ActivitiesFeedbackChartData>();

            var recipients = data
                .Select(x => x.Recipient ?? "Unknown")
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var reasons = data
                .Select(x => x.ActivityFeedbackReason ?? "Unknown")
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
                        (x.ActivityFeedbackReason ?? "Unknown") == reason);

                    return new GetActivitiesFeedback.ActivitiesFeedbackChartData
                    {
                        Recipient = recipient,
                        ActivityFeedbackReason = reason,
                        Count = match?.Count ?? 0
                    };
                }).ToList()
            }).ToList();
        }
    }
    
    private class ChartSeries
    {
        public string Reason { get; init; } = null!;
        public List<GetActivitiesFeedback.ActivitiesFeedbackChartData> Items { get; set; } = new();
    }
    
    private ApexChartOptions<GetActivitiesFeedback.ActivitiesFeedbackChartData> Options
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
    
    private async Task MarkAsRead(GetActivitiesFeedback.ActivitiesFeedbackTabularData item)
    {
        IsUpdating = true;

        var command = new MarkActivityFeedbackAsRead.Command
        {
            ActivityFeedbackId = item.Id
        };
        
        var result = await Service.Send(command);
        
        if (result.Succeeded)
        {
            item.IsRead = true; // instant UI update
            Snackbar.Add("Message read", Severity.Info);
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }

        IsUpdating = false;
    }
    
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
            _ = RefreshAsync(); // <-- forces the query to reload
        }
    }
}