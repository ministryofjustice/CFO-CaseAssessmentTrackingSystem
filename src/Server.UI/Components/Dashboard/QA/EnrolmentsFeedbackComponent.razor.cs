
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Components.Authorization;
using ApexCharts;
using Cfo.Cats.Application.Features.Participants.Commands;
using ChartType = ApexCharts.ChartType;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Components.Dashboard.QA;

public partial class EnrolmentsFeedbackComponent
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

    private bool HasReadItems => Data?.TabularData?.Any(x => x.IsRead) ?? false;
    
    protected override IRequest<Result<GetEnrolmentsFeedback.EnrolmentsFeedbackDto>> CreateQuery()
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

    private List<GetEnrolmentsFeedback.EnrolmentsFeedbackChartData> PieDataByReason
    {
        get
        {
            var data = Data?.ChartData ?? Array.Empty<GetEnrolmentsFeedback.EnrolmentsFeedbackChartData>();

            return data
                .GroupBy(x => x.EnrolmentFeedbackReason ?? "Unknown")
                .OrderBy(g => g.Key)
                .Select(g => new GetEnrolmentsFeedback.EnrolmentsFeedbackChartData
            {
                EnrolmentFeedbackReason = g.Key,
                Count = g.Sum(x => x.Count)
            })
            .ToList();
        }
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
    
    private ApexChartOptions<GetEnrolmentsFeedback.EnrolmentsFeedbackChartData> Options
        => new()
        {
            Chart = new Chart
            {
                Type = ChartType.Pie
            },
            Theme = new Theme
            {
                Mode = IsDarkMode ? Mode.Dark : Mode.Light
            }
        };
    
    private async Task MarkAsRead(GetEnrolmentsFeedback.EnrolmentsFeedbackTabularData item)
    {
        IsUpdating = true;

        var command = new MarkEnrolmentFeedbackAsRead.Command
        {
            EnrolmentFeedbackId = item.Id
        };
        
        var result = await Service.Send(command);
        
        if (result.Succeeded)
        {
            item.IsRead = true; // instant UI update
            StateHasChanged();
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
            StateHasChanged();
        }
    }
    
    private Color GetUnreadColour(DateTime? created, bool isRead)
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
}