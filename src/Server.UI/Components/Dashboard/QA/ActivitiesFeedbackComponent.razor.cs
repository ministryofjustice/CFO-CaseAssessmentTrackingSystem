
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Components.Authorization;
using ApexCharts;
using Cfo.Cats.Application.Features.Activities.Commands;
using ChartType = ApexCharts.ChartType;
using Color = MudBlazor.Color;

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

    private bool HasReadItems => Data?.TabularData?.Any(x => x.IsRead) ?? false;
    
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

    private List<GetActivitiesFeedback.ActivitiesFeedbackChartData> PieDataByReason
    {
        get
        {
            var data = Data?.ChartData ?? Array.Empty<GetActivitiesFeedback.ActivitiesFeedbackChartData>();

            return data
                .GroupBy(x => x.ActivityFeedbackReason ?? "Unknown")
                .OrderBy(g => g.Key)
                .Select(g => new GetActivitiesFeedback.ActivitiesFeedbackChartData
            {
                ActivityFeedbackReason = g.Key,
                Count = g.Sum(x => x.Count)
            })
            .ToList();
        }
    }
    
    
    private IEnumerable<GetActivitiesFeedback.ActivitiesFeedbackTabularData> FilteredData
    {
        get
        {
            var data = Data?.TabularData ?? Array.Empty<GetActivitiesFeedback.ActivitiesFeedbackTabularData>();

            if (ShowRead)
            {
                return data;
            }

            return data.Where(x => !x.IsRead);
        }
    }
    
    private ApexChartOptions<GetActivitiesFeedback.ActivitiesFeedbackChartData> Options
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