using Microsoft.AspNetCore.SignalR.Client;

namespace Cfo.Cats.Server.UI.Hubs;

/// <summary>
/// Scoped, per-circuit client that maintains a single connection to the <see cref="PresenceHub"/>.
/// Simply having this connection open is what marks the current user as online; there is no
/// client-facing API beyond starting and disposing it.
/// </summary>
public sealed class PresenceHubClient : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private bool _started;

    public PresenceHubClient(IHubConnectionFactory hubConnectionFactory)
    {
        _hubConnection = hubConnectionFactory.CreateForCurrentUser(PresenceHub.HubUrl);
        _hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
        _hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(10);

        _hubConnection.On<string, string>(PresenceHub.UserOnline, (userId, name) =>
        {
            UserOnline?.Invoke(userId, name);
            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Raised when a user the current user is permitted to see comes online. The arguments are
    /// the new user's id and display name.
    /// </summary>
    public event Action<string, string>? UserOnline;

    /// <summary>Opens the connection. Safe to call repeatedly; only the first call connects.</summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_started)
        {
            return;
        }

        _started = true;
        await _hubConnection.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _hubConnection.StopAsync().ConfigureAwait(false);
        }
        finally
        {
            await _hubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
