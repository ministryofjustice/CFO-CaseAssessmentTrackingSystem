using Microsoft.AspNetCore.SignalR.Client;

namespace Cfo.Cats.Server.UI.Hubs;

/// <summary>
/// Builds a <see cref="HubConnection"/> to an in-process hub, authenticated as the current
/// user by forwarding their identity cookie.
/// </summary>
public interface IHubConnectionFactory
{
    HubConnection CreateForCurrentUser(string relativeHubUrl);
}
