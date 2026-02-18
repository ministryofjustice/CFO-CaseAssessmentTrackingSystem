using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class SessionTimeoutDisplay
{

    [Inject]
    public IConfiguration Configuration { get; set; } = null!;

    [CascadingParameter] 
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    private TimeSpan? _remainingTime;
    private string? _userId;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _userId = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _cts = new CancellationTokenSource();

        if (string.IsNullOrEmpty(_userId) == false)
        {
            var tick = Configuration["SessionTick"] switch
            {
                null => 1,
                var s => int.TryParse(s, out var i) ? i : 1
            };

            _timer = new PeriodicTimer(TimeSpan.FromSeconds(tick));

            _ = RunTimerAsync(_cts.Token);
        }
    }

    private async Task RunTimerAsync(CancellationToken cancellationToken)
    {
        try
        {
            while(cancellationToken.IsCancellationRequested == false && _timer is not null && await _timer.WaitForNextTickAsync(cancellationToken))
            {
                await UpdateRemainingTime();
            }
        }
        catch (OperationCanceledException) 
        { 
            /* expected on dispose */ 
        }
    }

    private async Task UpdateRemainingTime()
    {
        _remainingTime = SessionService.GetRemainingSessionTime(_userId);
        await InvokeAsync(StateHasChanged);
    }

    private static string FormatTime(TimeSpan timeSpan)
        => timeSpan.TotalSeconds <= 0
            ? "Session Expired"
            : timeSpan.TotalMinutes >= 2
                ? timeSpan.ToString(@"mm\:ss")
                : $"{(int)timeSpan.TotalSeconds}s";

    private Color GetChipColor()
    => _remainingTime switch
    {
        { TotalMinutes: <= 2 } => Color.Warning,
        { TotalMinutes: <= 5 } => Color.Tertiary,
        _ => Color.Secondary
    };

    public async ValueTask DisposeAsync()
    {
        try
        {
            _cts?.Cancel();
        }
        catch { /* ignore */ }

        _cts?.Dispose();
        _timer?.Dispose();

        await Task.Yield();
    }

    private async Task TimerClicked()
    {
        SessionService.UpdateActivity(_userId);
        await UpdateRemainingTime();
    }

}