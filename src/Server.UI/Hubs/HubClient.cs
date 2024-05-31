using System.Net;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cfo.Cats.Server.UI.Hubs;

public sealed class HubClient : IAsyncDisposable
{
    public delegate Task MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    private readonly HubConnection hubConnection;
    private bool started;

    public HubClient(NavigationManager navigationManager, IHttpContextAccessor httpContextAccessor)
    {
        var uri = new UriBuilder(navigationManager.Uri);
        var container = new CookieContainer();
        if (httpContextAccessor.HttpContext != null)
        {
            foreach (var c in httpContextAccessor.HttpContext.Request.Cookies)
            {
                container.Add(
                    new Cookie(c.Key, c.Value)
                    {
                        Domain = uri.Host, // Set the domain of the cookie
                        Path = "/" // Set the path of the cookie
                    }
                );
            }
        }

        var hubUrl = navigationManager.BaseUri.TrimEnd('/') + ISignalRHub.Url;
        hubConnection = new HubConnectionBuilder()
            .WithUrl(
                hubUrl,
                options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                    options.Cookies = container;
                }
            )
            .WithAutomaticReconnect()
            .Build();

        hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);

        hubConnection.On<string, string>(
            nameof(ISignalRHub.Connect),
            (connectionId, userName) =>
                LoginEvent?.Invoke(this, new UserStateChangeEventArgs(connectionId, userName))
        );

        hubConnection.On<string, string>(
            nameof(ISignalRHub.Disconnect),
            (connectionId, userName) =>
                LogoutEvent?.Invoke(this, new UserStateChangeEventArgs(connectionId, userName))
        );

        hubConnection.On<string>(
            nameof(ISignalRHub.Start),
            message => JobStartedEvent?.Invoke(this, message)
        );

        hubConnection.On<string>(
            nameof(ISignalRHub.Completed),
            message => JobCompletedEvent?.Invoke(this, message)
        );

        hubConnection.On<string>(
            nameof(ISignalRHub.SendNotification),
            message => NotificationReceivedEvent?.Invoke(this, message)
        );

        hubConnection.On<string, string>(
            nameof(ISignalRHub.SendMessage),
            (from, message) =>
            {
                MessageReceivedEvent?.Invoke(this, new MessageReceivedEventArgs(from, message));
            }
        );

        hubConnection.On<string, string, string>(
            nameof(ISignalRHub.SendPrivateMessage),
            (from, to, message) =>
            {
                MessageReceivedEvent?.Invoke(this, new MessageReceivedEventArgs(from, message));
            }
        );
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await hubConnection.StopAsync();
        }
        finally
        {
            await hubConnection.DisposeAsync();
        }
    }

    public event EventHandler<UserStateChangeEventArgs>? LoginEvent;
    public event EventHandler<UserStateChangeEventArgs>? LogoutEvent;
    public event EventHandler<string>? JobStartedEvent;
    public event EventHandler<string>? JobCompletedEvent;
    public event EventHandler<string>? NotificationReceivedEvent;
    public event MessageReceivedEventHandler? MessageReceivedEvent;

    public async Task StartAsync(CancellationToken cancellation = default)
    {
        if (started)
        {
            return;
        }

        started = true;
        await hubConnection.StartAsync(cancellation);
    }

    public async Task SendAsync(string message)
    {
        await hubConnection.SendAsync(nameof(ISignalRHub.SendMessage), message);
    }

    public async Task NotifyAsync(string message)
    {
        await hubConnection.SendAsync(nameof(ISignalRHub.SendNotification), message);
    }
}
